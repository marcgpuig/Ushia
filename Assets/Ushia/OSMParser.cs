using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class OSMParser
{
    XmlDocument xmlData = null;

    public void load(string rawData)
    {
        XmlDocument xmlData = new XmlDocument();
        xmlData.LoadXml(rawData);
    }

    // <bounds minlat = "41.4823600" minlon="2.1307800" maxlat="41.4859300" maxlon="2.1345400"/>
    public int getMinLat() { return int.Parse(xmlData.SelectSingleNode("/osm/bounds").Attributes["minlat"].Value); }
    public int getMinLon() { return int.Parse(xmlData.SelectSingleNode("/osm/bounds").Attributes["minlon"].Value); }
    public int getMaxLat() { return int.Parse(xmlData.SelectSingleNode("/osm/bounds").Attributes["maxlat"].Value); }
    public int getMaxLon() { return int.Parse(xmlData.SelectSingleNode("/osm/bounds").Attributes["maxlon"].Value); }

    // TODO
    public List<OSMNode> getRoads()
    {
        List<OSMNode> l = new List<OSMNode>();

        return l;
    }

    public List<OSMNode> getNodes()
    {
        List<OSMNode> l = new List<OSMNode>();

        float ampliator = 10f;
        
        /*
        <osm>
            <node id="1423405850" visible="true" version="1" changeset="9212407" timestamp="2011-09-04T20:47:26Z" user="cfaerber" uid="17085" lat="48.1405398" lon="11.5430526"/>
        </osm>
        */

        XmlNodeList nodeList = xmlData.SelectNodes("/osm/node");
        //Debug.Log(nodeList.Count + " nodes in total.");

        OSMNode OSMn = new OSMNode();

        foreach (XmlNode n in nodeList)
        {
            OSMn.id  =   int.Parse(n.Attributes["id" ].Value);
            OSMn.lat = float.Parse(n.Attributes["lat"].Value) * ampliator;
            OSMn.lon = float.Parse(n.Attributes["lon"].Value) * ampliator;
            l.Add(OSMn);
        }

        return l;
    }
}
