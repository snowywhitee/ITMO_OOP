
fun main () {
    //some test code
    val iniParser = INIparser(fileName = "src/example.ini")
    iniParser.load()

    iniParser.getValueType("ADC_DEV", "BufferLenSeconds", "Double")
    iniParser.edit("ADC_DEV", "BufferLenSeconds", "0.25")
    iniParser.getValue("ADC_DEV", "BufferLenSeconds")

    iniParser.putToFile("src/out.ini")

}

