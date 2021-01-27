using GLTF.Math;
using GLTF.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLTF.Schema
{
	public class MMP_LightsImageBasedExtension : IExtension
	{
		public List<MMPImageBasedLight> Lights;

		public MMP_LightsImageBasedExtension()
		{
			Lights = new List<MMPImageBasedLight>();
		}

		public IExtension Clone(GLTFRoot gltfRoot)
		{
			var clone = new MMP_LightsImageBasedExtension();
			for (int i = 0; i < Lights.Count; i++)
			{
				clone.Lights.Add(new MMPImageBasedLight(Lights[i], gltfRoot));
			}
			return clone;
		}

		public JProperty Serialize()
		{
			JTokenWriter writer = new JTokenWriter();
			writer.WriteStartArray();
			for (int i = 0; i < Lights.Count; i++)
			{
				Lights[i].Serialize(writer);
			}
			writer.WriteEndArray();

			return new JProperty(MMP_LightsImageBasedExtensionFactory.EXTENSION_NAME,
				new JObject(
					new JProperty(MMP_LightsImageBasedExtensionFactory.PNAME_LIGHTS, writer.Token)
				)
			);
		}
	}

	public class MMPImageBasedLight : GLTFChildOfRootProperty
	{

		private const string PNAME_NAME = "name";
		private const string PNAME_ROTATION = "rotation";
		private const string PNAME_INTENSITY = "intensity";
		private const string PNAME_IRRADIANCE_COEFFICIENTS = "irradianceCoefficients";
		private const string PNAME_SPECULAR_IMAGE = "specularImage";
		private const string PNAME_SPECULAR_IMAGE_SIZE = "specularImageSize";

		public static readonly Quaternion ROTATION_DEFAULT = Quaternion.Identity;
		public static readonly double INTENSITY_DEFAULT = 1d;

		/// <summary>
		/// Name of the light
		/// </summary>
		public string LightName = null;

		/// <summary>
		/// Quaternion that represents the rotation of the IBL environment.
		/// </summary>
		public Quaternion Rotation = ROTATION_DEFAULT;

		/// <summary>
		/// Brightness multiplier for environment.
		/// </summary>
		public double Intensity = INTENSITY_DEFAULT;

		/// <summary>
		/// Declares spherical harmonic coefficients for irradiance up to l=2. This is a 9x3 array.
		/// </summary>
		public double[][] IrradianceCoefficients;

		public ImageId SpecularImage;

		public int SpecularImageSize;

		public MMPImageBasedLight()
		{

		}

		public MMPImageBasedLight(MMPImageBasedLight light, GLTFRoot gltfRoot) : base(light, gltfRoot)
		{

			LightName = light.LightName != null ? light.Name : null;
			Rotation = light.Rotation;
			Intensity = light.Intensity;
			IrradianceCoefficients = light.IrradianceCoefficients;
			SpecularImage = light.SpecularImage;
			SpecularImageSize = light.SpecularImageSize;

		}

		public override void Serialize(JsonWriter writer)
		{

			writer.WriteStartObject();

			if (LightName != null)
			{
				writer.WritePropertyName(PNAME_NAME);
				writer.WriteValue(LightName);
			}

			// ROTATION
			if (Rotation != Quaternion.Identity)
			{
				writer.WritePropertyName(PNAME_ROTATION);
				writer.WriteStartArray();
				writer.WriteValue(Rotation.X);
				writer.WriteValue(Rotation.Y);
				writer.WriteValue(Rotation.Z);
				writer.WriteValue(Rotation.W);
				writer.WriteEndArray();
			}


			// INTENSITY
			writer.WritePropertyName(PNAME_INTENSITY);
			writer.WriteValue(Intensity);


			// IRRADIANCE_COEFFICIENTS
			writer.WritePropertyName(PNAME_IRRADIANCE_COEFFICIENTS);
			writer.WriteStartArray();
			for (var x = 0; x < 9; x++)
			{
				writer.WriteStartArray();
				writer.WriteValue(IrradianceCoefficients[x][0]);
				writer.WriteValue(IrradianceCoefficients[x][1]);
				writer.WriteValue(IrradianceCoefficients[x][2]);
				writer.WriteEndArray();
			}
			writer.WriteEndArray();

			// SPECULAR_IMAGE
			writer.WritePropertyName(PNAME_SPECULAR_IMAGE);
			writer.WriteValue(SpecularImage.Id);

			//PNAME_SPECULAR_IMAGE_SIZE
			writer.WritePropertyName(PNAME_SPECULAR_IMAGE_SIZE);
			writer.WriteValue(SpecularImageSize);


			if (Extras != null)
			{
				writer.WritePropertyName("extras");
				Extras.WriteTo(writer);
			}

		}

	}


	public class MMPImageBasedLightId : GLTFId<MMPImageBasedLight>
	{
		public MMPImageBasedLightId()
		{
		}

		public MMPImageBasedLightId(MMPImageBasedLightId id, GLTFRoot newRoot) : base(id, newRoot)
		{
		}

		public override MMPImageBasedLight Value
		{
			get
			{
				if (Root.Extensions.TryGetValue(EXT_LightsImageBasedExtensionFactory.EXTENSION_NAME, out IExtension iextension))
				{
					MMP_LightsImageBasedExtension extension = iextension as MMP_LightsImageBasedExtension;
					return extension.Lights[Id];
				}
				else
				{
					throw new Exception("MMP_lights_image_based not found on root object");
				}
			}
		}

		public static MMPImageBasedLightId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new MMPImageBasedLightId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}


	}


	public class MMP_LightsImageBasedSceneExtension : IExtension
	{
		public MMPImageBasedLightId LightId;

		public MMP_LightsImageBasedSceneExtension()
		{

		}

		public MMP_LightsImageBasedSceneExtension(MMPImageBasedLightId lightId, GLTFRoot gltfRoot)
		{
			LightId = lightId;
		}

		public MMP_LightsImageBasedSceneExtension(int lightId, GLTFRoot gltfRoot)
		{
			LightId = new MMPImageBasedLightId
			{
				Id = lightId,
				Root = gltfRoot
			};
		}


		public IExtension Clone(GLTFRoot root)
		{
			return new MMP_LightsImageBasedSceneExtension(LightId.Id, root);
		}

		public JProperty Serialize()
		{
			return new JProperty(MMP_LightsImageBasedExtensionFactory.EXTENSION_NAME,
				new JObject(
					new JProperty(MMP_LightsImageBasedExtensionFactory.PNAME_LIGHT, LightId.Id)
				)
			);
		}

	}

}
