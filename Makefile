NAME = NoteStatistics

SOURCES = \
	NoteStatisticsAddin.cs \
	NoteStatsDialog.cs \
	gtk-gui/Tomboy.NoteStatistics.NoteStatsDialog.cs \
	gtk-gui/generated.cs


PACKAGES = \
	-pkg:tomboy-addins \
	-pkg:gtk-sharp-2.0

ASSEMBLIES = \
	-r:Mono.Posix \
	-r:System

RESOURCES = \
	-resource:$(NAME).addin.xml \
	-resource:gtk-gui/gui.stetic

ASSEMBLY = $(NAME).dll

all: $(ASSEMBLY)

install: all
	cp *.dll ~/.config/tomboy/addins/

mpack: $(ASSEMBLY)
	mautil p $(ASSEMBLY)

$(ASSEMBLY): $(SOURCES) $(NAME).addin.xml
	gmcs -unsafe -target:library -out:$(ASSEMBLY) $(SOURCES) $(PACKAGES) $(ASSEMBLIES) $(RESOURCES)

clean:
	rm -f *.dll *~ *.bak .mpack

PHONY:
	clean all mpack
