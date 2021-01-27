using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLTF.Schema
{
	public class MMP_LightsImageBasedExtensionFactory : ExtensionFactory
	{

		public const string EXTENSION_NAME = "MMP_lights_image_based";

		public const string PNAME_LIGHTS = "lights";

		public const string PNAME_LIGHT = "light";

		public MMP_LightsImageBasedExtensionFactory()
		{
			ExtensionName = EXTENSION_NAME;
		}

		public override IExtension Deserialize(GLTFRoot root, JProperty extensionToken)
		{
			return new MMP_LightsImageBasedExtension();
		}

	}
}
