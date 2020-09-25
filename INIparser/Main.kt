
fun main () {
    //some test code
    val iniParser = INIparser(fileName = "src/example2.ini")
    iniParser.load()

    println("${iniParser.getValueType("ADC_DEV", "BufferLenSeconds", "Double")}")
    iniParser.edit("ADC_DEV", "BufferLenSeconds", "0.25")
    println("${iniParser.getValue("ADC_DEV", "BufferLenSeconds")}")

    iniParser.putToFile("src/out.ini")

}

