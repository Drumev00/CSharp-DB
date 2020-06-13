using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniORM
{
	internal class ChangeTracker<T>
		where  T: class, new()
	{
		public IReadOnlyCollection<T> AllEntities => this.allEntities.AsReadOnly();
		public IReadOnlyCollection<T> Added => this.added.AsReadOnly();
		public IReadOnlyCollection<T> Removed => this.removed.AsReadOnly();

		private readonly List<T> allEntities;
		private readonly List<T> added;
		private readonly List<T> removed;

		public ChangeTracker(IEnumerable<T> entities)
		{
			this.added = new List<T>();
			this.removed = new List<T>();

			this.allEntities = CloneEntities(entities);
		}

		public void Add(T item) => this.added.Add(item);
		public void Remove(T item) => this.removed.Add(item);

		public IEnumerable<T> GetModifiedEntities(DbSet<T> dbSet)
		{
			List<T> modifiedEntities = new List<T>();

			PropertyInfo[] primaryKeys = typeof(T).GetProperties()
				.Where(pi => pi.HasAttribute<KeyAttribute>())
				.ToArray();

			foreach (T proxyEntity in this.AllEntities)
			{
				var primaryKeyValues = GetPrimaryKeyValues(primaryKeys, proxyEntity).ToArray();

				var entity = dbSet.Entities.Single(e => GetPrimaryKeyValues(primaryKeys, e).SequenceEqual(primaryKeyValues));

				var isModified = IsModified(proxyEntity, entity);
				if(isModified)
				{
					modifiedEntities.Add(entity);
				}
			}
			return modifiedEntities;
		}

		private static List<T> CloneEntities(IEnumerable<T> entities)
		{
			List<T> clonedEntities = new List<T>();

			PropertyInfo[] propsToClone = typeof(T).GetProperties()
				.Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType))
				.ToArray();

			foreach (var entity in entities)
			{
				T clonedEntity = Activator.CreateInstance<T>();

				foreach (var prop in propsToClone)
				{
					object value = prop.GetValue(entity);
					prop.SetValue(clonedEntity, value);
				}
				clonedEntities.Add(clonedEntity);
			}
			return clonedEntities;
		}

		private static bool IsModified(T proxyEntity, T entity)
		{
			PropertyInfo[] monitoredProps = typeof(T).GetProperties()
				.Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType))
				.ToArray();

			PropertyInfo[] modifiedProps = monitoredProps.
				Where(pi => !Equals(pi.GetValue(proxyEntity), pi.GetValue(entity)))
				.ToArray();

			return modifiedProps.Any();
		}
		
		private static IEnumerable<object> GetPrimaryKeyValues(IEnumerable<PropertyInfo> primaryKeys, T entity)
		{
			return primaryKeys.Select(pk => pk.GetValue(entity));
		}
	}
}