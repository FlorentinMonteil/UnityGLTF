using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLTF.Schema
{

	public class EXT_LightsImageBasedExtensionFactory : ExtensionFactory
	{

		public const string EXTENSION_NAME = "EXT_lights_image_based";

		public const string PNAME_LIGHTS = "lights";

		public const string PNAME_LIGHT = "light";

		public EXT_LightsImageBasedExtensionFactory()
		{
			ExtensionName = EXTENSION_NAME;
		}

		public override IExtension Deserialize(GLTFRoot root, JProperty extensionToken)
		{
			return new EXT_LightsImageBasedExtension();
		}
	}

}
