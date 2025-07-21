using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace ReferenceTable
{
    
    
    public partial class RefMap : RefDataItem
    {
        
		public int id {get; set;}
        
		public int map_id {get; set;}
        
		public string room_preset {get; set;}
        
        public override void Deserialize(ByteBuffer br)
        {
			id = br.ReadInteger();
			map_id = br.ReadInteger();
			room_preset = br.ReadString();
;
        }
    }
}
