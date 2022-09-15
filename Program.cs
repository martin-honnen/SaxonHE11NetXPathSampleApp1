using net.sf.saxon.s9api;
using net.liberty_development.SaxonHE11s9apiExtensions;
using System.Reflection;
using org.w3c.dom.xpath;

// force loading of updated xmlresolver
ikvm.runtime.Startup.addBootClassPathAssembly(Assembly.Load("org.xmlresolver.xmlresolver"));
ikvm.runtime.Startup.addBootClassPathAssembly(Assembly.Load("org.xmlresolver.xmlresolver_data"));

var processor = new Processor(false);

Console.WriteLine($"{processor.getSaxonEdition()} {processor.getSaxonProductVersion()}");

var xml = @"<items>
  <item>
   <name>item 1</name>
   <category>foo</category>
  </item>
  <item>
   <name>item 2</name>
   <category>bar</category>
  </item>
  <item>
   <name>item 3</name>
   <category>foo</category>
  </item>
</items>";

var xpath = @"
map:merge(
    /items/item ! map { string(category) : string(name) },
    map { 'duplicates' : 'combine' }
) 
=> map:for-each(function($k, $v) { map { $k : array { $v } } })
=> map:merge()";

var xpathCompiler = processor.newXPathCompiler();
xpathCompiler.declareNamespace("map", "http://www.w3.org/2005/xpath-functions/map");

var xpathResult = xpathCompiler.evaluate(xpath, processor.newDocumentBuilder().build(xml.AsSource()));

//simple output
Console.WriteLine(xpathResult);


//formatted output
var jsonSerializer = processor.NewSerializer(Console.Out);
jsonSerializer.setOutputProperty(Serializer.Property.METHOD, "json");
jsonSerializer.setOutputProperty(Serializer.Property.INDENT, "yes");

processor.writeXdmValue(xpathResult, jsonSerializer);



