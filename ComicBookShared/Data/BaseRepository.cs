using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    // the abstract keyword wont allow it to be directly instantiated
    // meaning this class should only be used as base class for our other repos
    // we define <TEntity> at class level so we dont have to define it again ad Add() and Update()
    // using the where keyword we can constrain our generic type parameter to reference types(like a class) Int or Boolean is value type
    // more info here https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters
    // BaseRepository<TEntity>
    // where TEntity : class
    public abstract class BaseRepository<TEntity>
        where TEntity : class
    {
        // this property stores the reference from the BaseRepository context Constructor below.
        protected Context Context { get; private set; }

        public BaseRepository(Context context)
        {
            Context = context;
        }
        // Get() is an generic method - GENERIC MEANS IT ACCEPT ANY DATA TYPE
        public abstract TEntity Get(int id, bool includeRelatedEntities = true);
        public abstract IList<TEntity> GetList();
        // Add and Update both accept an entity instance, since it's generic
        public void Add(TEntity entity)
        {
            // EFDB contex class that our Context inherites from, gives us acces to Set()
            // that we can call to get an reference to a DbSet object for provided entity type
            Context.Set<TEntity>().Add(entity);
            Context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            // Dbcontext Entry method gets use the passed in entity
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        // This is the stub entity approach
        public void Delete(int id)
        {
            var set = Context.Set<TEntity>();
            var entity = set.Find(id);
            set.Remove(entity);
            Context.SaveChanges();
        }
    }
}
