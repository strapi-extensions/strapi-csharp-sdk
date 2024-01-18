using BetterCoding.Strapi.SDK.Core.Services;
using System.Reflection;

namespace BetterCoding.Strapi.SDK.Core.Entry
{
    public class StrapiEntryClassController: IStrapiEntryClassController
    {
        static string ReservedStrapiEntryClassName { get; } = "_StrapiEntry";

        ReaderWriterLockSlim Mutex { get; } = new ReaderWriterLockSlim { };

        IDictionary<string, StrapiEntryClass> Classes { get; } = new Dictionary<string, StrapiEntryClass> { };

        Dictionary<string, Action> RegisterActions { get; set; } = new Dictionary<string, Action> { };

        public StrapiEntryClassController() => AddValid(typeof(StrapiEntry));

        public string GetClassName(Type type) => type == typeof(StrapiEntry) ? ReservedStrapiEntryClassName : type.GetStrapiEntryName();

        public Type GetType(string className)
        {
            Mutex.EnterReadLock();
            Classes.TryGetValue(className, out StrapiEntryClass info);
            Mutex.ExitReadLock();

            return info?.TypeInfo.AsType();
        }

        public bool GetClassMatch(string className, Type type)
        {
            Mutex.EnterReadLock();
            Classes.TryGetValue(className, out StrapiEntryClass subclassInfo);
            Mutex.ExitReadLock();

            return subclassInfo is { } ? subclassInfo.TypeInfo == type.GetTypeInfo() : type == typeof(StrapiEntry);
        }

        public void AddValid(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            if (!typeof(StrapiEntry).GetTypeInfo().IsAssignableFrom(typeInfo))
                throw new ArgumentException("Cannot register a type that is not a subclass of StrapiEntry");

            string className = GetClassName(type);

            try
            {
                // Perform this as a single independent transaction, so we can never get into an
                // intermediate state where we *theoretically* register the wrong class due to a
                // TOCTTOU bug.

                Mutex.EnterWriteLock();

                if (Classes.TryGetValue(className, out StrapiEntryClass previousInfo))
                    if (typeInfo.IsAssignableFrom(previousInfo.TypeInfo))
                        // Previous subclass is more specific or equal to the current type, do nothing.

                        return;
                    else if (previousInfo.TypeInfo.IsAssignableFrom(typeInfo))
                    {
                        // Previous subclass is parent of new child, fallthrough and actually register this class.
                        /* Do nothing */
                    }
                    else
                        throw new ArgumentException($"Tried to register both {previousInfo.TypeInfo.FullName} and {typeInfo.FullName} as the StrapiEntry subclass of {className}. Cannot determine the right class to use because neither inherits from the other.");

#warning Constructor detection may erroneously find a constructor which should not be used.

                ConstructorInfo constructor = type.FindConstructor() ?? type.FindConstructor(typeof(string), typeof(IServiceHub));

                if (constructor is null)
                    throw new ArgumentException("Cannot register a type that does not implement the default constructor!");

                Classes[className] = new StrapiEntryClass(type, constructor);
            }
            finally
            {
                Mutex.ExitWriteLock();
            }

            Mutex.EnterReadLock();
            RegisterActions.TryGetValue(className, out Action toPerform);
            Mutex.ExitReadLock();

            toPerform?.Invoke();
        }

        public void RemoveClass(Type type)
        {
            Mutex.EnterWriteLock();
            Classes.Remove(GetClassName(type));
            Mutex.ExitWriteLock();
        }

        public void AddRegisterHook(Type type, Action action)
        {
            Mutex.EnterWriteLock();
            RegisterActions.Add(GetClassName(type), action);
            Mutex.ExitWriteLock();
        }

        public StrapiEntry Instantiate(string className, IServiceHub serviceHub)
        {
            Mutex.EnterReadLock();
            Classes.TryGetValue(className, out StrapiEntryClass info);
            Mutex.ExitReadLock();

            return info is { } ? info.Instantiate().Bind(serviceHub) : new StrapiEntry(className, serviceHub);
        }

        public IDictionary<string, string> GetPropertyMappings(string className)
        {
            Mutex.EnterReadLock();
            Classes.TryGetValue(className, out StrapiEntryClass info);

            if (info is null)
                Classes.TryGetValue(ReservedStrapiEntryClassName, out info);

            Mutex.ExitReadLock();
            return info.PropertyMappings;
        }

        bool SDKClassesAdded { get; set; }

        // ALTERNATE NAME: AddObject, AddType, AcknowledgeType, CatalogType

        public void AddIntrinsic()
        {
            if (!(SDKClassesAdded, SDKClassesAdded = true).SDKClassesAdded)
            {

            }
        }
    }
}
