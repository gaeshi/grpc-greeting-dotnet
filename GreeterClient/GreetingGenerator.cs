using System.Collections.Generic;
using System.Reflection;
using Greet;

namespace GreeterClient
{
    public static class GreetingGenerator<T> where T : new()
    {
        public static IEnumerable<T> GenerateRequests()
        {
            var greetingProp = typeof(T).GetProperty("Greeting", BindingFlags.Public | BindingFlags.Instance);
            return new List<T>
            {
                GreetingRequest(greetingProp, new Greeting {FirstName = "Sheldon", LastName = "Cooper"}),
                GreetingRequest(greetingProp, new Greeting {FirstName = "Leonard", LastName = "Hofstadter"}),
                GreetingRequest(greetingProp, new Greeting {FirstName = "Howard", LastName = "Wolowitz"}),
                GreetingRequest(greetingProp, new Greeting {FirstName = "Raj", LastName = "Koothrappali"})
            };
        }

        private static T GreetingRequest(PropertyInfo greetingProp, Greeting greeting)
        {
            var item = new T();
            greetingProp.SetValue(item, greeting);
            return item;
        }
    }
}