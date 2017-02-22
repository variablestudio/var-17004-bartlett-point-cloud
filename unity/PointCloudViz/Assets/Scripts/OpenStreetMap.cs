using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class ShowOnlyAttribute : PropertyAttribute
{
}

public class OpenStreetMap : MonoBehaviour {

	public string accessToken = "pk.eyJ1IjoidmFyaWFibGVzdHVkaW8iLCJhIjoiY2l6OXdmMzkxMDAyMDJ3bzE4ZmVyd3R1ZiJ9.X0E6Rg1h2zsebOvCKbTKcA";
	public float lat = 51.52565f;
	public float lon = -0.08675f;
	public int zoom = 18;
	public int radius = 1;
	[ShowOnlyAttribute]
	public float TileSize;

	public WWW loadTileImage(int xOffset, int yOffset) {
		int x = (int)((lon + 180.0f) / 360.0f * (1 << zoom));
		int y = (int)((1.0f - Mathf.Log (Mathf.Tan (lat * Mathf.PI / 180.0f) +
			1.0f / Mathf.Cos (lat * Mathf.PI / 180.0f)) / Mathf.PI) / 2.0f * (1 << zoom));

		int tileX = x + xOffset;
		int tileY = y + yOffset;
		string url = "https://api.mapbox.com/v4/mapbox.streets/" + zoom + "/" + tileX + "/" + tileY + ".png?access_token=" + accessToken;
		Debug.Log("OSM " + url);
		return this.getCachedWWW (url);
	}

	// http://answers.unity3d.com/questions/330489/will-www-cache-image-files.html
	public WWW getCachedWWW (string url)
	{
		string filePath = Application.persistentDataPath;
		filePath += "/" + url.GetHashCode ();
		string loadFilepath = filePath;
		bool web = false;
		WWW www;
		bool useCached = false;
		useCached = System.IO.File.Exists (filePath);
		if (useCached) {
			//check how old
			System.DateTime written = File.GetLastWriteTimeUtc (filePath);
			System.DateTime now = System.DateTime.UtcNow;
			double totalHours = now.Subtract (written).TotalHours;
			if (totalHours > 300)
				useCached = false;
		}
		if (System.IO.File.Exists (filePath)) {
			string pathforwww = "file://" + loadFilepath;
			Debug.Log ("TRYING FROM CACHE " + url + "  file " + pathforwww);
			www = new WWW (pathforwww);
		} else {
			web = true;
			www = new WWW (url);
		}
		this.StartCoroutine (doLoad (www, filePath, web));
		return www;
	}

	static IEnumerator doLoad (WWW www, string filePath, bool web)
	{
		yield return www;

		if (www.error == null) {
			if (web) {
				//System.IO.Directory.GetFiles
				Debug.Log ("SAVING DOWNLOAD  " + www.url + " to " + filePath);
				// string fullPath = filePath;
				File.WriteAllBytes (filePath, www.bytes);
				Debug.Log ("SAVING DONE  " + www.url + " to " + filePath);
				//Debug.Log("FILE ATTRIBUTES  " + File.GetAttributes(filePath));
				//if (File.Exists(fullPath))
				// {
				//    Debug.Log("File.Exists " + fullPath);
				// }
			} else {
				Debug.Log ("SUCCESS CACHE LOAD OF " + www.url);
			}
		} else {
			if (!web) {
				File.Delete (filePath);
			}
			Debug.Log ("WWW ERROR " + www.error);
		}
	}

	//http://wiki.openstreetmap.org/wiki/Zoom_levels
	// original equation is S=C*cos(y)/2^(z+8)
	// but that calculates meters per pixel, each tile is 256px = 2^8 so we can remove 8
	// Fixed code from http://gis.stackexchange.com/questions/12991/how-to-calculate-distance-to-ground-of-all-18-osm-zoom-levels
	public float TileSizeInMeters () {
		float cosLat = Mathf.Cos(this.lat / 180 * Mathf.PI);
		float earthRadius = 6378137;
		float C = Mathf.PI * 2 * earthRadius; // equatorialCircumference
		return C * cosLat / Mathf.Pow(2, this.zoom);
	}


	public Vector2 GetTilePosition(float lon, float lat) {
		int zoom = this.zoom;
		float x = ((lon + 180.0f) / 360.0f * (1 << zoom));
		float y = ((1.0f - Mathf.Log (Mathf.Tan (lat * Mathf.PI / 180.0f) +
			1.0f / Mathf.Cos (lat * Mathf.PI / 180.0f)) / Mathf.PI) / 2.0f * (1 << zoom));
		
		return new Vector2(x, y);
	}

	float Tile2Lon(int x, int zoom) {
		return x / Mathf.Pow(2.0f, zoom) * 360.0f - 180;
	}

	float Tile2Lat(int y, int zoom) {
		double n = Mathf.PI - (2.0f * Mathf.PI * y) / Mathf.Pow(2.0f, zoom);
		return Mathf.Rad2Deg * Mathf.Atan((float)Math.Sinh(n));
	}

	public Vector2 GetPosition(float lon, float lat) {
		Vector2 origin = GetTilePosition(this.lon, this.lat);
		origin.x = Mathf.Floor(origin.x);
		origin.y = Mathf.Floor(origin.y);
		Vector2 pos = GetTilePosition(lon, lat);
		Vector2 realPos = (pos - origin - new Vector2(0.5f, 0.5f)) * TileSizeInMeters();
		realPos.y *= -1.0f; // UV flipping strikes back
		return realPos;
	}

	IEnumerator Start () {
		Material tileMaterial = Resources.Load ("Tile", typeof(Material)) as Material;
		float planeSize = 10; // default Unity plane is 10x10
		float scale = this.TileSizeInMeters() / (planeSize);

		Debug.Log("Tile size " + this.TileSizeInMeters() + " radius: " + radius);

		for (int i = -radius; i <= radius; i++) {
			for (int k = -radius; k <= radius; k++) {
				GameObject tile = GameObject.CreatePrimitive (PrimitiveType.Plane);
				tile.transform.position = new Vector3 (i * planeSize * scale, 0, -k * planeSize * scale);
				// need to flip it as UV 0,0 in for the unity plane is at TopRight instead of BottomLeft???
				tile.transform.localScale = new Vector3 (-scale, 1, -scale); //0.99 for gaps between tiles
				tile.transform.parent = this.transform;

				Renderer renderer = tile.GetComponent<Renderer> ();
				renderer.material = tileMaterial;

				WWW www = this.loadTileImage(i, k);
				yield return www;

				renderer.material.mainTexture = www.texture;
			}
		}

		TileSize = this.TileSizeInMeters();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
