﻿using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class BaseRecordMap : TableMap<BaseRecord>
    {
		public BaseRecordMap()
		{
			AutoMap();
		}
    }
}