GraphDB
	Database engine...store + query data
	Index data stored here

DataStore
	Object or entity data stored here

Graph
	Intermediate data object to visualize data and relationships


To use:

1. Define entities
	Simple poco classes that must inherit from Entity
	Ensure constructor is available

2. Define relationships
	Must inherit from Relationship
	Must implement abstract Create() with builder operations
		Build-->Entity-->Relates-->Constraint*-->Entity

3. Create or use data
	Each defined data point should be wrapped in Graph object
	Use Add<T>() to build related data
	Merge() with GraphDB
		Data already merged will be updated

4. Peform complex queries to access filtered data
	Use Get() with Filter 
		StartWith + (Where or Related)*
		Data returned is IEnumerable<Graph>
		Only final matched entities are returned and NOT complete hierarchies
		Further selection can be done from standard linq style query