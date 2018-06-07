function helloWorld() {
    return "Hello, World (from a function)!";
}

function add(num1, num2) {
    return num1 + num2;
}

function getProperty(jObject, property) {
    return eval("jObject."+property);
}