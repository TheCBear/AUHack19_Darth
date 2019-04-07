SOURCES=main.cpp protocol.cpp
OBJECTS=$(SOURCES:.cpp=.o)
DEPS=$(SOURCES:.cpp=.d)
EXE=boxClientRpi
CXXFLAGS=-I. -pthread -L/usr/local/boost/boost_1_39_0/stage/lib
LIBS=
ARCH=target

# Making for target
# > make ARCH=host
ifeq (${ARCH},host)
CXX=g++
BUILD_DIR=build/host/
EXEPATH=bin/host/
endif

# Making for target
# > make ARCH=target
ifeq (${ARCH},target)
CXX=arm-rpizw-g++
BUILD_DIR=build/target/
EXEPATH=bin/target/
endif

# Check the $(DEPS) new dependecy >>
$(addprefix $(EXEPATH),$(EXE)): $(addprefix $(BUILD_DIR),$(DEPS) $(OBJECTS))
	@mkdir -p $(EXEPATH)
	$(CXX) $(CXXFLAGS) -o $@ $(addprefix $(BUILD_DIR),$(OBJECTS)) ${LIBS}

# Rule that describes how a .d (dependency) file is created from a .cpp file
# Similar to the assignment that you just completed %.cpp -> %.o
$(addprefix $(BUILD_DIR),%.d): %.cpp
	@mkdir -p $(BUILD_DIR)
	$(CXX) -MT$(@:.d=.o) -MM $(CXXFLAGS) $^ > $@ ${LIBS}

# Rule for object files
$(addprefix $(BUILD_DIR),%.o): %.cpp
	@mkdir -p $(BUILD_DIR)
	$(CXX) $(CXXFLAGS) -c $^ -o $@ ${LIBS}


.PHONY: clean
clean:
	$(RM) $(addprefix $(EXEPATH),$(EXE)) $(addprefix $(BUILD_DIR),$(OBJECTS) $(DEPS))

ifneq ($(MAKECMDGOALS),clean)
-include $(DEPS)
endif
