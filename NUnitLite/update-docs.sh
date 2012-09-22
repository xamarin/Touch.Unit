# create documentation from the built-in (and shipped) version of MonoTouch.NUnitLite.dll
mdoc update --delete -o docs/en /Developer/MonoTouch/usr/lib/mono/2.1/MonoTouch.NUnitLite.dll

# remove internal stuff (i.e. not meant to be used for creating tests)
rm -rf docs/en/Mono.Options
rm -rf docs/en/NUnit.Framework.Internal
rm -rf docs/en/NUnit.Framework.Internal.Commands
rm -rf docs/en/NUnit.Framework.Internal.Filters
rm -rf docs/en/NUnitLite
rm -rf docs/en/NUnitLite.Runner
rm -rf docs/en/MonoTouch.NUnit
rm -rf docs/en/MonoTouch.NUnit.UI

rm -f	docs/en/ns-NUnit.Framework.Internal.*xml 	\
	docs/en/ns-NUnitLite.xml 			\
	docs/en/ns-NUnitLite.Runner.xml  		\
	docs/en/ns-Mono.Options.xml 			\
	docs/en/ns-MonoTouch.NUnit.xml 			\
	docs/en/ns-MonoTouch.NUnit.UI.xml

# filter out what we removed from index.xml
export INDEX="docs/en/index.xml" && csharp ./filter-index
