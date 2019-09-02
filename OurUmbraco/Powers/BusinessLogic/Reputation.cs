﻿using System.Collections;
using System.Xml;
using umbraco.cms.businesslogic.member;

namespace OurUmbraco.Powers.BusinessLogic {
    public class Reputation {

        public int Current { get; set; }
        public int Total { get; set; }
        public int MemberId { get; set; }
                
        public void Save() {
            if (MemberId > 0) {
                Member m = Library.Utils.GetMember(MemberId);
                m.getProperty(config.GetKey("/configuration/reputation/currentPointsAlias")).Value = Current;
                m.getProperty(config.GetKey("/configuration/reputation/totalPointsAlias")).Value = Total;
                
                m.XmlGenerate(new XmlDocument());
                m.Save();

                //make sure a cached member gets updated.
                Hashtable mems = Member.CachedMembers();
                if (mems.ContainsKey(m.Id)) {
                    mems.Remove(m.Id);
                    mems.Add(m.Id, m);
                }

            }
        }

        public Reputation(int memberId){
            Member m = Library.Utils.GetMember(memberId);

            if (m != null) {
                Current = obToInt( m.getProperty(config.GetKey("/configuration/reputation/currentPointsAlias")).Value );
                Total = obToInt( m.getProperty(config.GetKey("/configuration/reputation/totalPointsAlias")).Value );
                MemberId = m.Id;
            }
        }

        private int obToInt(object val) { 
            int retval = 0;
            int.TryParse(val.ToString(), out retval);

            return retval;          
        }

        public XmlNode ToXml(XmlDocument d) {

            XmlNode tx = d.CreateElement("reputation");

            tx.AppendChild(umbraco.xmlHelper.addTextNode(d, "current", Total.ToString()));
            tx.AppendChild(umbraco.xmlHelper.addCDataNode(d, "total", Current.ToString()));

            return tx;
        }
    }
}
