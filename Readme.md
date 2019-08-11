# Grammar Engine C# API

This project provides a fully managed C# API for the Russian Grammar Dictionary [GrammarEngine](https://github.com/Koziev/GrammarEngine).
This project uses a fork of Grammar Engine from [this repo](https://github.com/olegtarasov/GrammarEngine). All native libraries are downloaded via NuGet.
Library is supported on Windows and Ubuntu (tested on 16.04 and 18.04). MacOs is not supported (PRs to allow building GrammarEngine under MacOs are welcome).

There is only x64 version, x32 is not supported.

# Using under Windows

Most of the dependencies are included. You just need to have the latest Visual C++ runtime. Download here: [x64](https://aka.ms/vs/16/release/vc_redist.x64.exe).

# Using under Ubuntu

You will have to install and build some dependencies before you can use the library. First, install liblbfgs:

```bash
cd ~
wget https://github.com/downloads/chokkan/liblbfgs/liblbfgs-1.10.tar.gz
tar -xvzf liblbfgs-1.10.tar.gz
cd liblbfgs-1.10
./configure
make
sudo make install
```

Then install crfsuite:

```bash
cd ~
wget https://github.com/downloads/chokkan/crfsuite/crfsuite-0.12.tar.gz
tar -xvzf crfsuite-0.12.tar.gz
cd crfsuite-0.12
./configure
make
sudo make install
```

Finally, install boost and other dependencies:

```bash
sudo apt-get install sqlite3 libsqlite3-dev libboost-all-dev libncurses-dev
```

## Ubuntu 16.04

There are no additional steps you need to take under 16.04.

## Ubuntu 18.04

You need to create additional links for boost libraries:

```bash
sudo ln -s libboost_regex.so libboost_regex.so.1.58.0
sudo ln -s libboost_thread.so libboost_thread.so.1.58.0
sudo ln -s libboost_system.so libboost_system.so.1.58.0
```