namespace Kundalini.Processor
{
    using System;
    using System.Collections.Generic;
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
            Native.JsRunScript(script, _currentSourceContext++, "", out var _);
        }

        public static void Add(int a, int b)
        {
            Add(JavaScriptValue.GlobalObject, a, b);
        }

        public static void Add(JavaScriptValue objectContext, int a, int b)
        {
            var jsArgs = new[]
            {
                objectContext,
                JavaScriptValue.FromInt32(a),
                JavaScriptValue.FromInt32(b)
            };

            Native.JsCallFunction(
                objectContext.GetProperty(JavaScriptPropertyId.FromString("add")), jsArgs,
                (ushort) jsArgs.Length, out var functionValue);

            Console.WriteLine(JsValueAsString(functionValue));
        }

        public static void SayHello()
        {
            SayHello(JavaScriptValue.GlobalObject);
        }
        
        public static void SayHello(JavaScriptValue objectContext)
        {
            Native.JsCallFunction(
                GetFunctionByName("helloWorld"),
                new[] { objectContext }, 1, out var functionValue);

            Console.WriteLine(JsValueAsString(functionValue));
        }

        public static void GetObjectProperty(string jsonText, string property)
        {
            GetObjectProperty(JavaScriptValue.GlobalObject, jsonText, property);
        }

        public static void GetObjectProperty(JavaScriptValue objectContext, string jsonText, string property)
        {
            var jsonObject = JavaScriptValue.GlobalObject.GetProperty(
                JavaScriptPropertyId.FromString("JSON"));
            var parse = jsonObject.GetProperty(
                JavaScriptPropertyId.FromString("parse"));

            var stringInput = JavaScriptValue.FromString(jsonText);
            var parsedInput = parse.CallFunction(JavaScriptValue.GlobalObject, stringInput);

            var jsArgs = new[]
            {
                objectContext,
                parsedInput,
                JavaScriptValue.FromString(property)
            };

            Native.JsCallFunction(
                objectContext.GetProperty(JavaScriptPropertyId.FromString("getProperty")), jsArgs,
                (ushort)jsArgs.Length, out var functionValue);

            Console.WriteLine(JsValueAsString(functionValue));
        }

        private static JavaScriptValue GetFunctionByName(string name)
        {
            return GetFunctionByName(name, JavaScriptValue.GlobalObject);
        }

        private static JavaScriptValue GetFunctionByName(string name,
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
