import java.io.File
import java.io.InputStream

class ParserException(message:String): Exception(message)

class INIparser (private var fileName: String) {

    private var items = mutableListOf<Field>()
    private var empty: Boolean = true

    //prints the file
    @Throws(ParserException::class)
    fun print() {
        if (! File(fileName).exists()) throw ParserException("File $fileName not found")
        if (File(fileName).extension != "ini") throw ParserException("File $fileName must be .ini")

        val inputStream: InputStream = File(fileName).inputStream()
        val lineList: MutableList<String> = mutableListOf<String>()

        inputStream.bufferedReader().useLines { lines -> lines.forEach { lineList.add(it) } }
        lineList.forEach { println(it) }
    }

    //print read ini file to the cmd
    @Throws(ParserException::class)
    fun printItems() {
        if (empty) throw ParserException("INIparser wasn't loaded. Items list is empty")

        for (item: Field in items) {
            item.print()
        }
    }

    @Throws(ParserException::class)
    fun edit(fieldName: String, key: String, newValue: String) {
        if (! findField(fieldName)) throw ParserException("Field $fieldName not found")
        
        for (item: Field in items) {
            if (item.getName() == fieldName) {
                item.edit(key, newValue)
                break
            }
        }
    }

    @Throws(ParserException::class)
    fun getValue(fieldName: String, key: String) {
        if (! findField(fieldName)) throw ParserException("Field $fieldName not found")
        
        for (item: Field in items) {
            if (item.getName() == fieldName) {
                item.getValue(key)
                break
            }
        }
    }

    @Throws(ParserException::class)
    fun getValueType(fieldName: String, key: String, type: String) {
        if (! findField(fieldName)) throw ParserException("Field $fieldName not found")

        for (item: Field in items) {
            if (item.getName() == fieldName) {
                item.getValueType(key, type)
                break
            }
        }
    }

    //loads parsed INI file to the vector of Fields
    @Throws(ParserException::class)
    fun load() {
        if (! File(fileName).exists()) throw ParserException("File $fileName not found")
        else if (File(fileName).extension != "ini") throw ParserException("File $fileName must be .ini")
        
        val inputStream: InputStream = File(fileName).inputStream()
        val lineList: MutableList<String> = mutableListOf<String>()

        inputStream.bufferedReader().useLines { lines -> lines.forEach { lineList.add(it) } }
        val iterator: MutableListIterator<String> = lineList.listIterator()

        var currentField = Field(fieldName = "default")

        for (item: String in iterator) {
            if (item == "" || item[0] == ';') {
                //ignore empty lines and comments
            }
            else if (item[0] == '[') {
                if (currentField.getName() != "default") {
                    items.add(currentField)
                }
                var i: Int = 1
                var c: Char = item[i]
                var fieldName: String = ""
                while (c != ']') {
                    fieldName += c.toString()
                    i++
                    c = item[i]
                }
                currentField = Field(fieldName)
            }
            else {
                val key: String = readKey(item)
                val value: String = readValue(item)
                currentField.add(key, value)
            }
        }
        if (currentField.getName() != "default") {
            items.add(currentField)
        }
        empty = false
    }

    //puts current configuration to the file, overwrites by default
    fun putToFile(outFileName: String) {
        if (empty) throw ParserException("INIparser wasn't loaded")
        if (! File(outFileName).exists()) throw ParserException("File $outFileName not found")
        if (File(outFileName).extension != "ini") throw ParserException("File $outFileName must be .ini")

        val file = File(outFileName)
        file.writeText(text = "; ini file made with INIparser\n\n")
        for (item: Field in items) {
            item.putToFile(outFileName)
            file.appendText(text = "\n")
        }
    }

    //updates ini file with the current configuration
    fun fileUpdate() {
        putToFile(fileName)
    }

    //helper functions
    private fun readKey(str: String): String {
        var key: String = ""
        var i: Int = 0
        while (str[i] != ' ') {
            key += str[i].toString()
            i++
        }
        return key
    }

    private fun readValue(str: String): String {
        var value: String = ""
        var i: Int = 0
        var spaceCount: Int = 0
        var flag: Int = 0
        while (i != str.length) {
            if (str[i] == ' ') spaceCount++
            if (spaceCount > 2) break
            if (flag == 1) value += str[i]
            if (spaceCount == 2) flag = 1
            i++
        }
        return value
    }

    private fun findField(fieldName: String): Boolean {
        for (item: Field in items) if (item.getName() == fieldName) return true
        return false
    }
}

