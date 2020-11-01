.PHONY: all
all: main

main: main.cpp
	g++ -o main main.cpp


.PHONY: install
install:
	mkdir -p binaries
	cp -p main binaries

.PHONY: clean 
clean:
	rm -f main