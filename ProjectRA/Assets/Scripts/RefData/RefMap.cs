using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace ReferenceTable
{
    
    
    public partial class RefMap : RefDataItem
    {
        
		public int map_id {get; set;}
        
		public int start_room_id {get; set;}
        
        public override void Deserialize(ByteBuffer br)
        {
			map_id = br.ReadInteger();
			start_room_id = br.ReadInteger();
;
        }
    }
}
