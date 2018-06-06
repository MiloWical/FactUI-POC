namespace Kundalini.Processor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using ChakraCoreHost.Hosting;

    public class HelloWorld
    {
        private static JavaScriptSourceContext _currentSourceContext;

        static HelloWorld()
        {
            _currentSourceContext = JavaScriptSourceContext.FromIntPtr(IntPtr.Zero);

            // Create a runtime. 
            Native.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out var runtime);

            // Create an execution context. 
            Native.JsCreateContext(runtime, out var context);

            // Now set the execution context as being the current one on this thread.
            Native.JsSetCurrentContext(context);
        }

        public static void LoadScript(string script)
        {
            // Run the script.
            Native.JsRunScript(script, _currentSourceContext++, "", out var result);
        }

        public static void Add(int a, int b, params int[] additionalAddends)
        {
            Add(JavaScriptValue.GlobalObject, a, b, additionalAddends);
        }

        public static void Add(JavaScriptValue objectContext, int a, int b, params int[] additionalAddends)
        {
            var arguments = new List<JavaScriptValue>
            {
                objectContext,
                JavaScriptValue.FromInt32(a),
                JavaScriptValue.FromInt32(b)
            };

            if (additionalAddends != null && additionalAddends.Length > 0)
                arguments.AddRange(additionalAddends.Select(JavaScriptValue.FromInt32));

            var jsArgs = arguments.ToArray();

            Native.JsCallFunction(
                JavaScriptValue.GlobalObject.GetProperty(JavaScriptPropertyId.FromString("add")), jsArgs,
                (ushort) jsArgs.Length, out var functionValue);

            Console.WriteLine(JsValueAsString(functionValue));
        }

        public static void SayHello()
        {
            Native.JsCallFunction(
                GetFunctionByName("helloWorld"),
                new[] { JavaScriptValue.GlobalObject }, 1, out var functionValue);

            Console.WriteLine(JsValueAsString(functionValue));
        }

        public static JavaScriptValue GetFunctionByName(string name)
        {
            return GetFunctionByName(name, JavaScriptValue.GlobalObject);
        }

        public static JavaScriptValue GetFunctionByName(string name,
            JavaScriptValue objectContext)
        {
            return objectContext.GetProperty(JavaScriptPropertyId.FromString(name));
        }

        public static string JsValueAsString(JavaScriptValue value)
        {
            Native.JsConvertValueToString(value, out var resultJsString);
            Native.JsStringToPointer(resultJsString, out var stringValue, out _);
            return Marshal.PtrToStringUni(stringValue);
        }

        private static JavaScriptValue GetJsPropertyAtIndex(JavaScriptValue jsObject, int index)
        {
            Native.JsIntToNumber(index, out var jsIndex);
            Native.JsGetIndexedProperty(jsObject, jsIndex, out var result);
            return result;
        }

        public static string[] GetKeys(JavaScriptValue jsObject)
        {
            Native.JsGetOwnPropertyNames(jsObject, out var propertyNames);
            var keys = new List<string>();
            var i = 0;

            var key = JsValueAsString(GetJsPropertyAtIndex(propertyNames, i++));

            while (key != null)
            {
                keys.Add(key);
                key = JsValueAsString(GetJsPropertyAtIndex(propertyNames, i++));
            }
            return keys.ToArray();
        }
    }
}
