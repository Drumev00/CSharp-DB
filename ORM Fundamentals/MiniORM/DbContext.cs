using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MiniORM
{
	public abstract class DbContext
	{
		private readonly DatabaseConnection databaseConnection;
		private readonly Dictionary<Type, PropertyInfo> dbSetProperties;

		internal static readonly Type[] AllowedSqlTypes =
		{
			typeof(string),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(decimal),
			typeof(bool),
			typeof(DateTime)
		};

		protected DbContext(string connectionString)
		{
			this.databaseConnection = new DatabaseConnection(connectionString);

			this.dbSetProperties = DiscoverDbSets();
			using(new ConnectionManager(this.databaseConnection))
			{
				InitializeDbSets();
			}
			MapAllRelations();
		}

		public void SaveChanges()
		{
			object[] dbSets = this.dbSetProperties
				.Select(pi => pi.Value.GetValue(this))
				.ToArray();

			foreach (IEnumerable<object> dbSet in dbSets)
			{
				var invalidEntities = dbSet
					.Where(e => !IsObjectValid(e))
					.ToArray();

				if (invalidEntities.Any())
				{
					throw new InvalidOperationException($"{invalidEntities.Length} Invalid Entities found in {dbSet.GetType().Name}!");
				}
			}
			using (new ConnectionManager(this.databaseConnection))
			{
				using (var transaction = this.databaseConnection.StartTransaction())
				{
					foreach (IEnumerable dbSet in dbSets)
					{
						Type dbSetType = dbSet.GetType().GetGenericArguments().First();

						var persistMethod = typeof(DbContext)
							.GetMethod("Persist", BindingFlags.Instance | BindingFlags.NonPublic)
							.MakeGenericMethod(dbSetType);
						try
						{
							persistMethod.Invoke(this, new object[] { dbSet });
						}
						catch (TargetInvocationException tie)
						{
							throw tie.InnerException;
						}
						catch (InvalidOperationException)
						{
							transaction.Rollback();
							throw;
						}
						catch(SqlException)
						{
							transaction.Rollback();
							throw;
						}
					}
					transaction.Commit();
				}
			}
		}

		private void Persist<T>(DbSet<T> dbSet)
			where T : class, new()
		{
			var tableName = GetTableName(typeof(T));

			string[] columns = this.databaseConnection.FetchColumnNames(tableName).ToArray();

			if(dbSet.ChangeTracker.Added.Any())
			{
				this.databaseConnection.InsertEntities(dbSet.ChangeTracker.Added, tableName, columns);
			}

			T[] modifiedEntities = dbSet.ChangeTracker.GetModifiedEntities(dbSet).ToArray();
			if (modifiedEntities.Any())
			{
				this.databaseConnection.UpdateEntities(modifiedEntities, tableName, columns);
			}
			if (dbSet.ChangeTracker.Removed.Any())
			{
				this.databaseConnection.DeleteEntities(dbSet.ChangeTracker.Removed, tableName, columns);
			}

		}

		private void InitializeDbSets()
		{
			foreach (var dbSet in dbSetProperties)
			{
				Type dbSetType = dbSet.Key;
				PropertyInfo dbSetProperty = dbSet.Value;

				MethodInfo populateDbSetGeneric = typeof(DbContext)
					.GetMethod("PopulateDbSet", BindingFlags.Instance | BindingFlags.NonPublic)
					.MakeGenericMethod(dbSetType);

				populateDbSetGeneric.Invoke(this, new object[] { dbSetProperty });
			}
		}

		private void PopulateDbSet<T>(PropertyInfo dbSet)
			where T : class, new()
		{
			var entities = LoadTableEntities<T>();

			var dbSetInstance = new DbSet<T>(entities);
			ReflectionHelper.ReplaceBackingField(this, dbSet.Name, dbSetInstance);
		}

		private void MapAllRelations()
		{
			foreach (var dbSetProp in this.dbSetProperties)
			{
				Type dbSetType = dbSetProp.Key;

				MethodInfo mapRelationsGeneric = typeof(DbContext)
					.GetMethod("MapRelations", BindingFlags.Instance | BindingFlags.NonPublic)
					.MakeGenericMethod(dbSetType);

				object dbSet = dbSetProp.Value.GetValue(this);

				mapRelationsGeneric.Invoke(this, new object[] { dbSet });
			}
		}

		private void MapRelations<T>(DbSet<T> dbSet)
			where T : class, new()
		{
			Type entityType = typeof(T);

			MapNavigationProperties(dbSet);

			PropertyInfo[] collections = entityType
				.GetProperties()
				.Where(pi =>
					pi.PropertyType.IsGenericType &&
					pi.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
				.ToArray();

			foreach (PropertyInfo collection in collections)
			{
				Type collectionType = collection.PropertyType.GetGenericArguments().First();

				MethodInfo mapCollectionMethod = typeof(DbContext)
					.GetMethod("MapCollection", BindingFlags.Instance | BindingFlags.NonPublic)
					.MakeGenericMethod(entityType, collectionType);

				mapCollectionMethod.Invoke(this, new object[] { dbSet, collection });
			}

		}

		private void MapCollection<TDbSet, TCollection>(DbSet<TDbSet> dbSet, PropertyInfo collectionProperty)
			where TDbSet : class, new()
			where TCollection : class, new()
		{
			Type entityType = typeof(TDbSet);
			Type collectionType = typeof(TCollection);

			PropertyInfo[] primaryKeys = collectionType.GetProperties()
				.Where(pi => pi.HasAttribute<KeyAttribute>())
				.ToArray();

			PropertyInfo primaryKey = primaryKeys.First();
			PropertyInfo foreignKey = entityType.GetProperties()
				.First(pi => pi.HasAttribute<KeyAttribute>());

			bool isManyToMany = primaryKeys.Length >= 2;
			if (isManyToMany)
			{
				primaryKey = collectionType.GetProperties()
					.First(pi => collectionType
							.GetProperty(pi.GetCustomAttribute<ForeignKeyAttribute>().Name)
							.PropertyType == entityType);
			}

			DbSet<TCollection> navigationDbSet = (DbSet<TCollection>)this.dbSetProperties[collectionType].GetValue(this);

			foreach (TDbSet entity in dbSet)
			{
				object primaryKeyValue = foreignKey.GetValue(entity);

				TCollection[] navigationEntities = navigationDbSet
					.Where(navigationEntity => primaryKey.GetValue(navigationEntity).Equals(primaryKeyValue))
					.ToArray();

				ReflectionHelper.ReplaceBackingField(entity, collectionProperty.Name, navigationEntities);
			}
		}

		private void MapNavigationProperties<TEntity>(DbSet<TEntity> dbSet)
			where TEntity : class, new()
		{
			Type entityType = typeof(TEntity);

			PropertyInfo[] foreignKeys = entityType.GetProperties()
				.Where(pi => pi.HasAttribute<ForeignKeyAttribute>())
				.ToArray();

			foreach (var fk in foreignKeys)
			{
				string navigationPropName = fk.GetCustomAttribute<ForeignKeyAttribute>().Name;
				PropertyInfo navigationProp = entityType.GetProperty(navigationPropName);

				object navigationDbSet = this.dbSetProperties[navigationProp.PropertyType]
					.GetValue(this);

				PropertyInfo navigationPrimaryKey = navigationProp.PropertyType.GetProperties()
					.First(pi => pi.HasAttribute<KeyAttribute>());

				foreach (var entity in dbSet)
				{
					object foreignKeyValue = fk.GetValue(entity);

					object navigationPropValue = ((IEnumerable<object>)navigationDbSet)
						.First(currentProp => navigationPrimaryKey.GetValue(currentProp).Equals(foreignKeyValue));

					navigationProp.SetValue(entity, navigationPropValue);
				}
			}
		}

		private bool IsObjectValid(object e)
		{
			var validationContext = new ValidationContext(e);
			var validationErrors = new List<ValidationResult>();

			return Validator.TryValidateObject(e, validationContext, validationErrors, true);
		}

		private IEnumerable<TEntity> LoadTableEntities<TEntity>()
			where TEntity : class, new()
		{
			Type table = typeof(TEntity);

			var columns = GetEntityColumnNames(table);

			var tableName = GetTableName(table);

			TEntity[] fetchRows = this.databaseConnection.FetchResultSet<TEntity>(tableName, columns).ToArray();

			return fetchRows;
		}

		private string GetTableName(Type tableType)
		{
			string tableName = ((TableAttribute)Attribute.GetCustomAttribute(tableType, typeof(TableAttribute)))?.Name;

			if (tableName == null)
			{
				tableName = this.dbSetProperties[tableType].Name;
			}

			return tableName;
		}

		private Dictionary<Type, PropertyInfo> DiscoverDbSets()
		{
			Dictionary<Type, PropertyInfo> dbSets = this.GetType().GetProperties()
				.Where(pi => pi.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
				.ToDictionary(pi => pi.PropertyType.GetGenericArguments().First(), pi => pi);

			return dbSets;
		}

		private string[] GetEntityColumnNames(Type table)
		{
			string tableName = this.GetTableName(table);
			var dbColumns = this.databaseConnection.FetchColumnNames(tableName);

			string[] columns = table.GetProperties()
				.Where(pi => dbColumns.Contains(pi.Name) &&
						!pi.HasAttribute<NotMappedAttribute>() &&
						AllowedSqlTypes.Contains(pi.PropertyType))
				.Select(pi => pi.Name)
				.ToArray();

			return columns;
		}
	}

}