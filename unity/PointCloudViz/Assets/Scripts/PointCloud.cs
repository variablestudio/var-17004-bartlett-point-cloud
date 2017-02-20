using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PointCloud : MonoBehaviour {

	public string file;
	[TextArea]
	public string matrix;
	private ParticleSystem ps;
	private ParticleSystem.Particle[] particles;
	public Transform player;
	public InfluenceZone influenceZone;

	static IEnumerator LoadTextFile(string file_path, Matrix4x4 transform, System.Action<ParticleSystem.Particle[]> callback) {
		Debug.Log("PointCloud.LoadTextFile: " + file_path);
		StreamReader inp_stm = new StreamReader(file_path);
		transform = transform.inverse;

		if (File.Exists(file_path + ".bin")) {
			Debug.Log("PointCloud.LoadTextFile: loading binary: " + file_path + ".bin");
			BinaryReader br = new BinaryReader(new FileStream(file_path + ".bin", FileMode.Open));
			int numParticles = br.ReadInt32();
			Debug.Log("PointCloud.LoadTextFile: numParticles: " + numParticles);
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
			for (int i = 0; (br.BaseStream.Position != br.BaseStream.Length); i++) {
				Vector3 position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
				position = new Vector3(position.x, position.z, position.y);
				position = transform.MultiplyVector(position);
				position = new Vector3(position.x, position.z, position.y);
				particles[i].position = position;
				particles[i].startColor = new Color32(br.ReadByte(), br.ReadByte(), br.ReadByte(), 255);
				particles[i].startSize = 0.2f;
			}
			br.Close();
			callback(particles);
			yield return null;
		} else {

			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000000];
			Bounds bbox = new Bounds();

			int lineNumber = 0;
			int numParticles = 0;
			while(!inp_stm.EndOfStream) {
				string line = inp_stm.ReadLine( );
				lineNumber++;

				if (file_path.EndsWith("ptx") && (lineNumber % 100 != 0)) continue;

				string[] entries = line.Split(' ');
				if (entries.Length > 0 && entries.Length == 7) {// && entries[0] == "v") {
					Vector3 position = new Vector3();
					Color color = new Color();
					if (file_path.EndsWith("ptx")) {
						position = new Vector3(float.Parse(entries[0]), float.Parse(entries[2]), float.Parse(entries[1]));
						color = new Color(float.Parse(entries[4]) / 255.0f, float.Parse(entries[5]) / 255.0f, float.Parse(entries[6]) / 255.0f, 1f);
					} else if (file_path.EndsWith("obj")) {
						//XZY
						position = new Vector3(float.Parse(entries[1]), float.Parse(entries[2]), float.Parse(entries[3]));
						position = transform.MultiplyPoint(position);
						//XYZ
						position = new Vector3(position.x, position.z, position.y);
						color = new Color(float.Parse(entries[4]), float.Parse(entries[5]), float.Parse(entries[6]), 1f);
					}

					particles[numParticles].position = position;
					particles[numParticles].startColor = color;
					particles[numParticles].startSize = 0.3f;
					if (numParticles == 1) {
						bbox = new Bounds(position, new Vector3(0.0f, 0.0f, 0.0f));
					}
					else {
						bbox.Encapsulate(new Bounds(position, new Vector3(0.0f, 0.0f, 0.0f)));
					}
					numParticles++;
					if (numParticles > particles.Length - 1) {
						break;
					}
				}

				if (lineNumber % 10000 == 0) {
					Debug.Log("PointCloud " + lineNumber + " : " + line + " numParticles:" + numParticles);
					yield return new WaitForSeconds (0.01f);
				}
			}
			Debug.Log("PointCloud.LoadTextFile: DONE numParticles:" + numParticles +  " bbox: " + bbox.ToString());

			BinaryWriter bw = new BinaryWriter(new FileStream(file_path + ".bin", FileMode.Create));
			bw.Write(numParticles);
			for (int i = 0; i < numParticles; i++) {
				bw.Write(particles[i].position.x);
				bw.Write(particles[i].position.y);
				bw.Write(particles[i].position.z);
				bw.Write(particles[i].startColor.r);
				bw.Write(particles[i].startColor.g);
				bw.Write(particles[i].startColor.b);
			}
			bw.Close();

			callback(particles);

			inp_stm.Close(); 
			yield return null;
		}	
	}

	void LoadFile () {
		Debug.Log("PointCloud LoadFile");
		Matrix4x4 transform = new Matrix4x4();
		float[] transformValues = Array.ConvertAll(this.matrix.Split(new Char[] {' ', '\n'}), Single.Parse);
		transform.SetRow(0, new Vector4(transformValues[0], transformValues[1], transformValues[2], transformValues[3]));
		transform.SetRow(1, new Vector4(transformValues[4], transformValues[5], transformValues[6], transformValues[7]));
		transform.SetRow(2, new Vector4(transformValues[8], transformValues[9], transformValues[10], transformValues[11]));
		transform.SetRow(3, new Vector4(transformValues[12], transformValues[13], transformValues[14], transformValues[15]));
		transform = transform.transpose;
		transform = transform.inverse;

		this.StartCoroutine(LoadTextFile(file, transform, (ParticleSystem.Particle[] particles) => {
			this.particles = particles;
			this.ps.SetParticles(particles, particles.Length);
		}));
	}

	// Use this for initialization
	void Start () {
		Debug.Log("PointCloud Start");

		this.ps = this.gameObject.AddComponent<ParticleSystem>() as ParticleSystem;
		this.ps.Stop();

		LoadFile();

		Material pointMaterial = Resources.Load ("PointRGB", typeof(Material)) as Material;
		//pointMaterial = Resources.Load ("PointHeight", typeof(Material)) as Material;
		this.GetComponent<ParticleSystemRenderer>().material = pointMaterial;
	}
	
	// Update is called once per frame
	void Update () {
		if (this.ps != null && this.particles != null) {
			this.ps.SetParticles(particles, particles.Length);
		}

		this.GetComponent<ParticleSystemRenderer>().material.SetVector("_Origin", new Vector4(player.position.x, player.position.y, player.position.z));
		this.GetComponent<ParticleSystemRenderer>().material.SetVector("_InfluenceOrigin",
			new Vector4(influenceZone.transform.position.x, influenceZone.transform.position.y, influenceZone.transform.position.z, influenceZone.radius));
		this.GetComponent<ParticleSystemRenderer>().material.SetVector("_InfluenceColor",influenceZone.GetComponent<Renderer>().material.GetColor("_Color"));

	}


	private int toolbarInt = 0;
	private int prevToolbarInt = 0;
	private string[] toolbarStrings = {"RGB", "Height", "Distance", "Influence"};

	void OnGUI () {
		toolbarInt = GUI.Toolbar (new Rect (25, 25, 250, 30), toolbarInt, toolbarStrings);
		if (toolbarInt > -1 && toolbarInt != prevToolbarInt) {
			prevToolbarInt = toolbarInt;
			Debug.Log("Change material to " + toolbarInt + " " + "Point" + toolbarStrings[toolbarInt]);
			Material pointMaterial = Resources.Load ("Point" + toolbarStrings[toolbarInt], typeof(Material)) as Material;
			this.GetComponent<ParticleSystemRenderer>().material = pointMaterial;
		}

		Event e = Event.current;
		if (e.keyCode == KeyCode.Alpha1) toolbarInt = 0;
		if (e.keyCode == KeyCode.Alpha2) toolbarInt = 1;
		if (e.keyCode == KeyCode.Alpha3) toolbarInt = 2;
		if (e.keyCode == KeyCode.Alpha4) toolbarInt = 3;
	}

}
