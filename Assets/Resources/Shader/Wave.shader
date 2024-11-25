Shader "Custom/Wave"
{
    Properties
    {
    _Color("Color",Color)=(1,1,1,1)
    _Radius("Radius",Float)=0.4
    _Thickness("Thickness",Float)=0.15
    _Center("Center",Vector)=(0.5,0.5,0,0)
    _Speed("Growth Speed", Float) = 0.2
    _Interval("Interval", Float) = 0.5
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Tags{ "LightMode" = "ForwardBase" }
			ZWrite Off  
			Blend SrcAlpha OneMinusSrcAlpha   
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"
            struct v2f{
                float4 vertex:SV_POSITION;
                float4 position:TEXCOORD1;
                float2 uv:TEXCOORD0;
            };

            v2f vert(appdata_base v){
                v2f o;
                o.vertex=UnityObjectToClipPos(v.vertex);
                o.position=v.vertex;
                o.uv=v.texcoord;
                return o;
            }

            float _Radius;
            float _Thickness;
            float _Speed;
            float _Interval;
            float4 _Center;
            fixed4 _Color;

            float circle(float2 uv,float2 center, float radius, float thickness){
                float2 offset=uv-center;
                float len=length(offset);
                return step(len,radius+thickness)-step(len,radius);
            } 
           

            fixed4 frag (v2f i) : SV_Target
            {
                _Radius = _Radius + _Thickness/2;
                float2 center = float2(_Center.x, _Center.y);
                float time = _Time.y * _Speed; 
                float currentRadius_1 = _Radius * (time % 1);
                float currentRadius_2 = _Radius * ((time + _Interval) % 1);
                float circleValue_1 = circle(i.uv, center, currentRadius_1, _Thickness);
                float circleValue_2 = circle(i.uv, center, currentRadius_2, _Thickness);
                float circleValue_3 = circle(i.uv, center, 0, 0.1);
            
                fixed4 col_1 = circleValue_1 * _Color;
                fixed4 col_2 = circleValue_2 * _Color;
                fixed4 col_3 = circleValue_3 * _Color;

                float2 offset = i.uv - center;
                float distance = length(offset);

                float alpha_1 = saturate((distance - currentRadius_1) / (_Thickness/1.2));
                float alpha_2 = saturate((distance - currentRadius_2) / (_Thickness/1.2));
                float alpha_3 = saturate(1.0 - (distance - 0) / 0.1);

                col_1.a *= alpha_1 * step(0.0, circleValue_1);
                col_2.a *= alpha_2 * step(0.0, circleValue_2);
                col_3.a *= alpha_3 * step(0.0, circleValue_3);
                fixed4 col = col_1 + col_2 + col_3;
                col.rgb = min(col.rgb, _Color.rgb);
                return col;
            }
            ENDCG
        }
    }
}