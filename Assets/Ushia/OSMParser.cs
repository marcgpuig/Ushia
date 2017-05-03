using System.Collections;
using System.Collections.Generic;
using System.Xml;

using UnityEngine;

public class OSMParser : UThreadWrapper
{
    private XmlDocument xmlData = null;
    private USlippyTile tile = null;

    public Hashtable nodes = null;
    public Hashtable ways = null;
    public Hashtable roads = null;

    /*public void load(string rawData)
    {
        xmlData.LoadXml(rawData);
    }*/

    public OSMParser(USlippyTile t) { tile = t; }

    /// <bounds minlat = "41.4823600" minlon="2.1307800" maxlat="41.4859300" maxlon="2.1345400"/>
    public double getMinLat() { return double.Parse(xmlData.SelectSingleNode("/osm/bounds").Attributes["minlat"].Value); }
    public double getMinLon() { return double.Parse(xmlData.SelectSingleNode("/osm/bounds").Attributes["minlon"].Value); }
    public double getMaxLat() { return double.Parse(xmlData.SelectSingleNode("/osm/bounds").Attributes["maxlat"].Value); }
    public double getMaxLon() { return double.Parse(xmlData.SelectSingleNode("/osm/bounds").Attributes["maxlon"].Value); }

    // TODO
    private List<OSMNode> getRoads()
    {
        List<OSMNode> l = new List<OSMNode>();

        return l;
    }

    private Hashtable getNodes()
    {
        /// node structure:
        /// <osm>
        ///     <node id="1423405850" lat="48.1405398" lon="11.5430526"/>
        ///     <node id="1463405850" lat="48.1872398" lon="11.5830526">
        ///         <tag k="waterway" v="river"/>
        ///     </node>
        /// </osm> 

        Hashtable nodes = new Hashtable();
        XmlNodeList nodeList = xmlData.SelectNodes("/osm/node");

        foreach (XmlNode n in nodeList)
        {
            long   id  =   long.Parse(n.Attributes["id" ].Value);
            double lat = double.Parse(n.Attributes["lat"].Value);
            double lon = double.Parse(n.Attributes["lon"].Value);
            OSMNode node = new OSMNode(id, lon, lat);
            
            /// adding all the node tags
            foreach (XmlNode tag in n.SelectNodes("tag"))
            {
                string k = tag.Attributes["k"].Value;
                string v = tag.Attributes["v"].Value;
                node.tags.Add(k, v);
            }

            nodes.Add(id, node);
        }
        return nodes;
    }

    private Hashtable getWays()
    {
        /// node structure:
        /// <osm>
        ///     <way id="35418976">
        ///         <nd ref= "1391558216"/>
        ///         <tag k="waterway" v="river"/>
        ///     </way>
        /// </osm>
        /// 
        Hashtable ways = new Hashtable();
        XmlNodeList wayList = xmlData.SelectNodes("/osm/way");

        foreach (XmlNode w in wayList)
        {
            long id = long.Parse(w.Attributes["id"].Value);
            OSMWay way = new OSMWay();

            /// adding all the node references
            foreach (XmlNode t in w.SelectNodes("nd"))
            {
                long nodeRef = long.Parse(t.Attributes["ref"].Value);
                way.nodesIds.Add(nodeRef);
            }

            /// adding all the way tags
            foreach (XmlNode t in w.SelectNodes("tag"))
            {
                string k = t.Attributes["k"].Value;
                string v = t.Attributes["v"].Value;
                way.tags.Add(k, v);
            }

            ways.Add(id, way);
        }
        return ways;
    }

    /// Threaded task. DON'T use the Unity API here!
    protected override void ThreadFunction()
    {
        xmlData = OSMDownloader.getOSMXML(tile);
        nodes = getNodes();
        ways = getWays();
        //roads = getRoads();
    }

    /// This is executed by the Unity main thread when the job is finished
    protected override void OnFinished()
    {
        Debug.Log("OSMParser " + tile + " done in " + (endTime - startTime).TotalSeconds + " sec.");
    }
}
