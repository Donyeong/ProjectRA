using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace ReferenceTable
{
    
    
    public partial class RefProp : RefDataItem
    {
        
		public int prop_id {get; set;}
        
		public string prop_name_key {get; set;}
        
		public int price {get; set;}
        
		public int mass {get; set;}
        
		public int drag {get; set;}
        
		public int air_drag {get; set;}
        
		public float damageMultiplier {get; set;}
        
		public int durability {get; set;}
        
		public int hp {get; set;}
        
		public eItemType item_type {get; set;}
        
        public override void Deserialize(ByteBuffer br)
        {
			prop_id = br.ReadInteger();
			prop_name_key = br.ReadString();
			price = br.ReadInteger();
			mass = br.ReadInteger();
			drag = br.ReadInteger();
			air_drag = br.ReadInteger();
			damageMultiplier = br.ReadFloat();
			durability = br.ReadInteger();
			hp = br.ReadInteger();
			item_type = (eItemType)br.ReadInteger();
;
        }
    }
}
