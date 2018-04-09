using Lippert.Core.Data;
using Lippert.Core.Tests.TestSchema.Contracts;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public abstract class CommonColumnMapBase<T> : TableMap<T>
		where T : IGuidIdentifier, ICreateEditFields
	{
		public CommonColumnMapBase()
		{
			Map(x => x.Id).Key();
		}

		protected void MapProperties()
		{
			Map(x => x.CreatedByUserId).Ignore(IgnoreBehavior.Update);
			Map(x => x.CreatedDateUtc).Generated();
			Map(x => x.ModifiedByUserId);
			Map(x => x.ModifiedDateUtc).Ignore(IgnoreBehavior.Insert);
		}
	}
}