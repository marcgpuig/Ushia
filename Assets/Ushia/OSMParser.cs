using System.Collections;
using System.Collections.Generic;
using System.Xml;

using UnityEngine;

public class OSMParser : UThreadWrapper
{
    private XmlDocument xmlData = null;
    private USlippyTile tile    = null;

    public double[] bounds = new double[4];
    public double width    = 0;
    public double height   = 0;

    public Hashtable nodes = null;
    public Hashtable ways  = null;

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

    private void getBounds()
    {
        bounds[0] = getMinLat();
        bounds[1] = getMinLon();
        bounds[2] = getMaxLat();
        bounds[3] = getMaxLon();
    }

    private void calculateDimensions()
    {
        width = UMaths.lon2x(bounds[3]) - UMaths.lon2x(bounds[1]);
        height = UMaths.lat2y(bounds[2]) - UMaths.lat2y(bounds[0]);
    }

    private void getNodes()
    {
        /// node structure:
        /// <osm>
        ///     <node id="1423405850" lat="48.1405398" lon="11.5430526"/>
        ///     <node id="1463405850" lat="48.1872398" lon="11.5830526">
        ///         <tag k="waterway" v="river"/>
        ///     </node>
        /// </osm> 

        nodes = new Hashtable();
        XmlNodeList nodeList = xmlData.SelectNodes("/osm/node");

        foreach (XmlNode n in nodeList)
        {
            double lat = double.Parse(n.Attributes["lat"].Value);
            double lon = double.Parse(n.Attributes["lon"].Value);

            /// if the node is outside the boundings don't add it
            if (lat <= bounds[2] && lat >= bounds[0] && lon < bounds[3] && lon > bounds[1])
            {
                long id = long.Parse(n.Attributes["id"].Value);

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
        }


    }

    private void getWays()
    {
        /// node structure:
        /// <osm>
        ///     <way id="35418976">
        ///         <nd ref= "1391558216"/>
        ///         [...]
        ///         <nd ref= "1391558253"/>
        ///         <tag k="waterway" v="river"/>
        ///     </way>
        /// </osm>
        /// 
        ways = new Hashtable();
        XmlNodeList wayList = xmlData.SelectNodes("/osm/way");

        foreach (XmlNode w in wayList)
        {
            long id = long.Parse(w.Attributes["id"].Value);
            OSMWay way = new OSMWay();

            OSMNode prev = null;
            OSMNode next = null;

            /// adding all the node references
            foreach (XmlNode t in w.SelectNodes("nd"))
            {
                long nodeRef = long.Parse(t.Attributes["ref"].Value);

                if (nodes.ContainsKey(nodeRef))
                {
                    OSMNode n = (OSMNode)nodes[nodeRef];
                    way.nodesIds.Add(nodeRef);
                    prev = n;
                }
            }

            /// adding all the way tags
            foreach (XmlNode t in w.SelectNodes("tag"))
            {
                string k = t.Attributes["k"].Value;
                string v = t.Attributes["v"].Value;
                way.tags.Add(k, v);
            }

            /*if ( way.tags.ContainsKey("highway")
                || way.tags.ContainsKey("waterway")
                || way.tags.ContainsKey("craft")
                || way.tags.ContainsKey("railway")
                || way.tags.ContainsKey("power") )*/
            ways.Add(id, way);            
        }

    }

    /// Threaded task. DON'T use the Unity API here!
    protected override void ThreadFunction()
    {
        xmlData = OSMDownloader.getOSMXML(tile);

        getBounds();
        calculateDimensions();
        getNodes();
        getWays();
    }

    /// This is executed by the Unity main thread when the job is finished
    protected override void OnFinished()
    {
        //Debug.Log("OSMParser: " + tile + "have " + nodes.Count + " nodes. Done in " + (endTime - startTime).TotalSeconds + " sec.");
    }
}