using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace ReferenceTable
{
    
    
    public partial class RefMonster : RefDataItem
    {
        
		public int room_id {get; set;}
        
		public int map_id {get; set;}
        
		public string room_preset {get; set;}
        
		public int weight {get; set;}
        
        public override void Deserialize(ByteBuffer br)
        {
			room_id = br.ReadInteger();
			map_id = br.ReadInteger();
			room_preset = br.ReadString();
			weight = br.ReadInteger();
;
        }
    }
}
