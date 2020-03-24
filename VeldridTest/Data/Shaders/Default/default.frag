#version 450

// Set: resource layout index
// Binding: element in layout description
layout(set = 0, binding = 0) uniform texture2D Input;
layout(set = 0, binding = 1) uniform sampler InputSampler;

layout(set = 1, binding = 0) buffer Uniforms {
    vec4 Clr;
};

layout(location = 0) in vec4 fsin_Color;
layout(location = 1) in vec2 fsin_UV;

layout(location = 0) out vec4 fsout_Color;

void main() {
    fsout_Color = texture(sampler2D(Input, InputSampler), fsin_UV);
    //fsout_Color = Clr;
}