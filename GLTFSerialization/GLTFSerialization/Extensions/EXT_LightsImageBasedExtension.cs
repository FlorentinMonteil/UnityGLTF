using GLTF.Extensions;
using GLTF.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace GLTF.Schema
{
	public class EXT_LightsImageBasedExtension : IExtension
	{

		public List<ImageBasedLight> Lights;

		public EXT_LightsImageBasedExtension()
		{
			Lights = new List<ImageBasedLight>();
		}

		public IExtension Clone(GLTFRoot gltfRoot)
		{
			var clone = new EXT_LightsImageBasedExtension();
			for (int i = 0; i < Lights.Count; i++)
			{
				clone.Lights.Add(new ImageBasedLight(Lights[i], gltfRoot));
			}
			return new EXT_LightsImageBasedExtension();
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

			return new JProperty(EXT_LightsImageBasedExtensionFactory.EXTENSION_NAME,
				new JObject(
					new JProperty(EXT_LightsImageBasedExtensionFactory.PNAME_LIGHTS, writer.Token)
				)
			);
		}
	}


	public class ImageBasedLight : GLTFChildOfRootProperty
	{

		private const string PNAME_NAME = "name";
		private const string PNAME_ROTATION = "rotation";
		private const string PNAME_INTENSITY = "intensity";
		private const string PNAME_IRRADIANCE_COEFFICIENTS = "irradianceCoefficients";
		private const string PNAME_SPECULAR_IMAGES = "specularImages";
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

		/// <summary>
		/// Declares an array of the first N mips of the prefiltered cubemap.Each mip is, in turn, defined with an array of 6 images, one for each cube face.i.e. this is an Nx6 array.
		/// </summary>

		public ImageId[][] SpecularImages;

		/// <summary>
		/// "The dimension (in pixels) of the first specular mip. This is needed to determine, pre-load, the total number of mips needed."
		/// </summary>
		public int SpecularImageSize;

		public ImageBasedLight()
		{

		}

		public ImageBasedLight(ImageBasedLight light, GLTFRoot gltfRoot) : base(light, gltfRoot)
		{

			LightName = light.LightName != null ? light.Name : null;
			Rotation = light.Rotation;
			Intensity = light.Intensity;
			IrradianceCoefficients = light.IrradianceCoefficients;
			SpecularImages = light.SpecularImages;
			SpecularImageSize = light.SpecularImageSize;

		}

		public override void Serialize(JsonWriter writer)
		{

			writer.WriteStartObject();

			if(LightName != null)
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

			// SPECULAR_IMAGES
			writer.WritePropertyName(PNAME_SPECULAR_IMAGES);
			writer.WriteStartArray();
			for (var x = 0; x < SpecularImages.Length; x++)
			{
				writer.WriteStartArray();
				for (var y = 0; y < 6; y++)
				{
					writer.WriteValue(SpecularImages[x][y].Id);
				}
				writer.WriteEndArray();
			}
			writer.WriteEndArray();

			//PNAME_SPECULAR_IMAGE_SIZE
			writer.WritePropertyName(PNAME_SPECULAR_IMAGE_SIZE);
			writer.WriteValue(SpecularImageSize);

		}

	}


	public class ImageBasedLightId : GLTFId<ImageBasedLight>
	{
		public ImageBasedLightId()
		{
		}

		public ImageBasedLightId(ImageBasedLightId id, GLTFRoot newRoot) : base(id, newRoot)
		{
		}

		public override ImageBasedLight Value
		{
			get
			{
				if (Root.Extensions.TryGetValue(EXT_LightsImageBasedExtensionFactory.EXTENSION_NAME, out IExtension iextension))
				{
					EXT_LightsImageBasedExtension extension = iextension as EXT_LightsImageBasedExtension;
					return extension.Lights[Id];
				}
				else
				{
					throw new Exception("EXT_lights_image_based not found on root object");
				}
			}
		}

		public static ImageBasedLightId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new ImageBasedLightId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}


	}

	public class EXT_LightsImageBasedSceneExtension : IExtension
    {
		public ImageBasedLightId LightId;

		public EXT_LightsImageBasedSceneExtension()
		{

		}

		public EXT_LightsImageBasedSceneExtension(ImageBasedLightId lightId, GLTFRoot gltfRoot)
		{
			LightId = lightId;
		}

		public EXT_LightsImageBasedSceneExtension(int lightId, GLTFRoot gltfRoot)
		{
			LightId = new ImageBasedLightId
			{
				Id = lightId,
				Root = gltfRoot
			};
		}


		public IExtension Clone(GLTFRoot root)
		{
			return new EXT_LightsImageBasedSceneExtension(LightId.Id, root);
		}

		public JProperty Serialize()
		{
			return new JProperty(EXT_LightsImageBasedExtensionFactory.EXTENSION_NAME,
				new JObject(
					new JProperty(EXT_LightsImageBasedExtensionFactory.PNAME_LIGHT, LightId.Id)
				)
			);
		}

	}

}
