using System.Runtime.Serialization;

namespace Amphasis.Azure.SimaLand.Models.Enums;

public enum SimaLandImageSize
{
	[EnumMember(Value = "140.jpg")]
	Quad140,

	[EnumMember(Value = "280.jpg")]
	Quad280,

	[EnumMember(Value = "400.jpg")]
	Quad400,

	[EnumMember(Value = "700-nw.jpg")]
	Quad700,

	[EnumMember(Value = "1600.jpg")]
	Quad1600Watermark
}