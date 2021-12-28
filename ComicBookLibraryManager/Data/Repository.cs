using ComicBookShared.Data;
using ComicBookShared.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace ComicBookLibraryManager.Data
{
    /// <summary>
    /// Repository class that provides various database queries
    /// and CRUD operations. EACH of the Ilists<> are needed by the view model for respective pages.
    /// RECAP, the repository class has a method to complete every one of the TODOs in the index, detail and add action methods
    /// </summary>
    public static class Repository
    {
        /// <summary>
        /// Private method that returns a database context.
        /// </summary>
        /// <returns>An instance of the Context class. 
        /// Other methods in this class call this method to get instance of context</returns>
        static Context GetContext()
        {
            var context = new Context();
            context.Database.Log = (message) => Debug.WriteLine(message);
            return context;
        }

        /// <summary>
        /// Returns a count of the comic books.
        /// </summary>
        /// <returns>An integer count of the comic books.</returns>
        public static int GetComicBookCount()
        {
            using (Context context = GetContext())
            {
                return context.ComicBooks.Count();
            }
        }

        /// <summary>
        /// Returns a list of comic books ordered by the series title 
        /// and issue number.
        /// </summary>
        /// <returns>An IList collection of ComicBook entity instances. 
        /// It eagerly loads the related series entity by including the series navigation property </returns>
        /// ToList() forces execution of the query. This returns a list collection type IList<T> and it makes it clear
        /// that the database query will happen here in this method.
        public static IList<ComicBook> GetComicBooks()
        {
            using (Context context = GetContext())
            {
                return context.ComicBooks
                    .Include(cb => cb.Series)
                    .OrderBy(cb => cb.Series.Title)
                    .ThenBy(cb => cb.IssueNumber)
                    .ToList();
            }
        }

        /// <summary>
        /// Returns a single comic book.
        /// </summary>
        /// <param name="comicBookId">The comic book ID to retrieve.</param>
        /// <returns>A fully populated ComicBook entity instance. Eagerrly loaded</returns>
        public static ComicBook GetComicBook(int comicBookId)
        {
            using (Context context = GetContext())
            {
                return context.ComicBooks
                    .Include(cb => cb.Series)
                    .Include(cb => cb.Artists.Select(a => a.Artist))
                    .Include(cb => cb.Artists.Select(a => a.Role))
                    .Where(cb => cb.Id == comicBookId)
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Returns a list of series ordered by title.
        /// </summary>
        /// <returns>An IList collection of Series entity instances.</returns>
        public static IList<Series> GetSeries()
        {
            using (Context context = GetContext())
            {
                return context.Series
                    .OrderBy(s => s.Title)
                    .ToList();
            }
        }

        /// <summary>
        /// Returns a single series.
        /// </summary>
        /// <param name="seriesId">The series ID to retrieve.</param>
        /// <returns>A Series entity instance.</returns>
        public static Series GetSeries(int seriesId)
        {
            using (Context context = GetContext())
            {
                return context.Series
                    .Where(s => s.Id == seriesId)
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Returns a list of artists ordered by name.
        /// </summary>
        /// <returns>An IList collection of Artist entity instances.</returns>
        public static IList<Artist> GetArtists()
        {
            using (Context context = GetContext())
            {
                return context.Artists
                    .OrderBy(a => a.Name)
                    .ToList();
            }
        }

        /// <summary>
        /// Returns a list of roles ordered by name.
        /// </summary>
        /// <returns>An IList collection of Role entity instances.</returns>
        public static IList<Role> GetRoles()
        {
            using (Context context = GetContext())
            {
                return context.Roles
                    .OrderBy(r => r.Name)
                    .ToList();
            }
        }

        /// <summary>
        /// Adds a comic book.
        /// </summary>
        /// <param name="comicBook">The ComicBook entity instance to add.</param>
        public static void AddComicBook(ComicBook comicBook)
        {
            using (Context context = GetContext())
            {
                context.ComicBooks.Add(comicBook);

                if (comicBook.Series != null && comicBook.Series.Id > 0)
                {
                    context.Entry(comicBook.Series).State = EntityState.Unchanged;
                }

                foreach (ComicBookArtist artist in comicBook.Artists)
                {
                    if (artist.Artist != null && artist.Artist.Id > 0)
                    {
                        context.Entry(artist.Artist).State = EntityState.Unchanged;
                    }

                    if (artist.Role != null && artist.Role.Id > 0)
                    {
                        context.Entry(artist.Role).State = EntityState.Unchanged;
                    }
                }

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates a comic book.
        /// </summary>
        /// <param name="comicBook">The ComicBook entity instance to update.</param>
        /// The UpdateComicBook() is past as an instance of comicBook entity, 
        /// but that entity is not being tracked by the context(also called detached or disconnected entities).
        /// 1 .First option to persist data is to retrieve(using Find()) the entity from the database, then manually update property values.
        /// 2 .Second option still uses Find() but we retrive entity Entry() from context and call current values
        /// 3 .Third and best option. Does not require two queries. attach the passed in entity to context, instead of retrieving it from context
        /// after attaching entity to context the state will be set to unchanged. EF cant detect current value from database
        /// but we can force EF to treat new entity values as new values by setting entry state to modified comicBookEntry.State = EntityState.Modified;
        /// 
        public static void UpdateComicBook(ComicBook comicBook)
        {
            using (Context context = GetContext())
            {
                // attach the passed in entity to context, instead of retrieving it from context
                context.ComicBooks.Attach(comicBook);
                var comicBookEntry = context.Entry(comicBook);
                comicBookEntry.State = EntityState.Modified;
                //comicBookEntry.Property("IssueNumber").IsModified = false;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a comic book.
        /// </summary>
        /// <param name="comicBookId">The comic book ID to delete.</param>
        public static void DeleteComicBook(int comicBookId)
        {
            using (Context context = GetContext())
            {
                var comicBook = new ComicBook() { Id = comicBookId };
                // Retrieve its entry from context and set it's state to deleted.
                context.Entry(comicBook).State = EntityState.Deleted;

                context.SaveChanges();
            }
        }
    }
}
