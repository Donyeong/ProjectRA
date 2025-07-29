using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace ReferenceTable
{
    
    
    public partial class RefPropSpawner : RefDataItem
    {
        
		public int key {get; set;}
        
		public int spawner_id {get; set;}
        
		public int prop_id {get; set;}
        
        public override void Deserialize(ByteBuffer br)
        {
			key = br.ReadInteger();
			spawner_id = br.ReadInteger();
			prop_id = br.ReadInteger();
;
        }
    }
}
