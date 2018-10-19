# Shom.S57

Shom.S57 is a suite of libraries for managing S57 files.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. 
See deployment for notes on how to deploy the project on a live system.

### About S57
S57 is the IHO standards for producing Electronic Nautical Charts (ENC).

In this format, geospatial data is stored as vectors and features.
Vectors are first described, then features.

#### Vectors
For a valid ENC, vectors are built using the chain-node method.
Vectors can be an isolated node, a connected node or an edge (composed of two connected nodes).
One important attribute for vectors is the positional accuracy (given as the POSACC attribute).

#### Features
Features carry all the data regarding objects on the map.
Each type of feature has its own list of attributes.

This library does not manage chart update files. 
Base S57 map files use a .000 extension.
Update files to apply to this base map file are subsequently numbered .001, .002, .003 and so on...

### Prerequisites
Base libraries Shom.ISO8211 and Shom.s57 are portable.

## Built With

* [Visual Studio 2017](https://visualstudio.microsoft.com/fr/downloads/) - Microsoft IDE for editing open source projects

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Arnaud Ménard** - *Initial work* - [Shom](http://www.shom.fr)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

