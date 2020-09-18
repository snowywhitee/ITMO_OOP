import java.io.File

class Field (private var fieldName: String) {

    private val items = mutableMapOf<String, Any>()
    private var empty: Boolean = true

    //adds a new key - value to the mutable map
    fun add(key: String, value: String) {
        items[key] = getType(value)
        empty = false
    }

    //edits values with the given key
    @Throws(ParserException::class)
    fun edit(key: String, value: String) {
        if (! items.containsKey(key)) throw ParserException("Key $key not found")

        //check the value type
        if (items[key] is Double && getType(value) is Double) {
            items[key] = getType(value)
        }
        else if (items[key] is Int && getType(value) is Int) {
            items[key] = getType(value)
        }
        else if (items[key] is String) {
            items[key] = value
        }
        else {
            throw ParserException("Types don't match")
        }

    }

    //prints value using key
    @Throws(ParserException::class)
    fun getValue(key: String) {
        if (! items.containsKey(key)) throw ParserException("Key $key not found")
        println("${items[key]}")
    }

    @Throws(ParserException::class)
    fun getValueType(key: String, type: String) {
        if (! items.containsKey(key)) throw ParserException("Key $key not found")

        when {
            type == "Int" && items[key] is Int -> println("${items[key]}")

            type == "Double" && items[key] is Double -> println("${items[key]}")

            type == "String" && items[key] is String -> println("${items[key]}")

            else -> throw ParserException("Desired type & value mismatch")
        }
    }

    //prints the items contents
    @Throws(ParserException::class)
    fun print() {
        if (empty) throw ParserException("Field '$fieldName' is empty")
        println("${fieldName}: " + items.toString())
    }

    //writes items contents to a file
    fun putToFile(outFileName: String) {
        if (! File(outFileName).exists()) throw ParserException("File $outFileName not found")
        if (File(outFileName).extension != "ini") throw ParserException("File $outFileName must be .ini")

        val file = File(outFileName)
        file.appendText(text = "[$fieldName]\n")
        for (item in items) {
            file.appendText(text = "${item.key} = ${item.value}\n")
        }
    }

    //helper functions
    fun getName(): String {
        return fieldName
    }

    private fun getType(value: String): Any {
        val intPattern = Regex(pattern = "^[-+]?(0|[1-9]|[1-9][1-9]*)$")
        val doublePattern = Regex(pattern = "^[-+]?[0-9]+\\.[0-9]+")

        return when {
            doublePattern.containsMatchIn(value) -> value.toDouble()

            intPattern.containsMatchIn(value) -> value.toInt()

            else -> value
        }

    }
}