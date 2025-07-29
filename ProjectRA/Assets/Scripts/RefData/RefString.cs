using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace ReferenceTable
{
    
    
    public partial class RefString : RefDataItem
    {
        
		public string string_key {get; set;}
        
		public string kr {get; set;}
        
        public override void Deserialize(ByteBuffer br)
        {
			string_key = br.ReadString();
			kr = br.ReadString();
;
        }
    }
}
