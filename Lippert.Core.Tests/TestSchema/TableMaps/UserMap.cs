using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class UserMap : TableMap<User>
	{
		public UserMap() { }
		public UserMap(string tableName) => Table(tableName);
		public UserMap(System.Type includePropertiesAssignableTo)
			: base(includePropertiesAssignableTo) { }
	}
}