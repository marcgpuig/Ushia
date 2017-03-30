using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class OSMParser
{
    XmlDocument xmlData = null;

    public OSMParser()
    {
        xmlData = new XmlDocument();
    }

    public void load(string rawData)
    {
        xmlData.LoadXml(rawData);
    }

    // <bounds minlat = "41.4823600" minlon="2.1307800" maxlat="41.4859300" maxlon="2.1345400"/>
    public double getMinLat() { return double.Parse(xmlData.SelectSingleNode("/osm/bounds").Attributes["minlat"].Value); }
    public double getMinLon() { return double.Parse(xmlData.SelectSingleNode("/osm/bounds").Attributes["minlon"].Value); }
    public double getMaxLat() { return double.Parse(xmlData.SelectSingleNode("/osm/bounds").Attributes["maxlat"].Value); }
    public double getMaxLon() { return double.Parse(xmlData.SelectSingleNode("/osm/bounds").Attributes["maxlon"].Value); }

    // TODO
    public List<OSMNode> getRoads()
    {
        List<OSMNode> l = new List<OSMNode>();

        return l;
    }

    public List<OSMNode> getNodes()
    {
        List<OSMNode> l = new List<OSMNode>();        
        /*
        <osm>
            <node id="1423405850" visible="true" version="1" changeset="9212407" timestamp="2011-09-04T20:47:26Z" user="cfaerber" uid="17085" lat="48.1405398" lon="11.5430526"/>
        </osm>
        */
        XmlNodeList nodeList = xmlData.SelectNodes("/osm/node");
        //Debug.Log(nodeList.Count + " nodes in total.");

        foreach (XmlNode n in nodeList)
        {
            long   id  =   long.Parse(n.Attributes["id" ].Value);
            double lat = double.Parse(n.Attributes["lat"].Value);
            double lon = double.Parse(n.Attributes["lon"].Value);
            l.Add(new OSMNode(id, lon, lat));
        }
        return l;
    }
}
