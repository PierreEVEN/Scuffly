#if !UNITY_2019_1_OR_NEWER
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
using System.Reflection;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    public class AIHDSubShader : IAIHDSubShader
	{
        Pass m_PassForward = new Pass()
        {
            Name = "Forward",
            LightMode = "Forward",
            TemplateName = "HDLitPass.template",
            MaterialName = "Lit",
            ShaderPassName = "SHADERPASS_FORWARD",
            // ExtraDefines are set when the pass is generated
            Includes = new List<string>()
            {
                //"#include \"Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassForward.hlsl\"",
            },
            RequiredFields = new List<string>()
            {
                "FragInputs.worldToTangent",
                "FragInputs.positionRWS",
                "FragInputs.texCoord1",
                "FragInputs.texCoord2"
            },
            PixelShaderSlots = new List<int>()
            {
				AIHDMasterNode.AlphaSlotId,
				AIHDMasterNode.AlphaThresholdSlotId,
				AIHDMasterNode.OutputRT0SlotId,
				AIHDMasterNode.OutputRT1SlotId,
				AIHDMasterNode.OutputRT2SlotId,
				AIHDMasterNode.OutputRT3SlotId,
				AIHDMasterNode.OutputRT4SlotId,
				AIHDMasterNode.OutputRT5SlotId,
				AIHDMasterNode.OutputRT6SlotId,
				AIHDMasterNode.OutputRT7SlotId,
			},
            VertexShaderSlots = new List<int>()
            {
                AIHDMasterNode.PositionSlotId
            },
            UseInPreview = true,

            OnGeneratePassImpl = (IMasterNode node, ref Pass pass) =>
            {
                var masterNode = node as AIHDMasterNode;
                pass.StencilOverride = new List<string>()
                {
                    "// Stencil setup",
                    "Stencil",
                    "{",
                    string.Format("   WriteMask {0}", (int) HDRenderPipeline.StencilBitMask.LightingMask),
                    string.Format("   Ref  {0}", masterNode.RequiresSplitLighting() ? (int)StencilLightingUsage.SplitLighting : (int)StencilLightingUsage.RegularLighting),
                    "   Comp Always",
                    "   Pass Replace",
                    "}"
                };

                pass.ExtraDefines.Remove("#define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST");
                //if (masterNode.surfaceType == SurfaceType.Opaque && masterNode.alphaTest.isOn)
                //{
                //    pass.ExtraDefines.Add("#define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST");
                //    pass.ZTestOverride = "ZTest Equal";
                //}
                //else
                {
                    pass.ZTestOverride = null;
                }

                if (masterNode.surfaceType == SurfaceType.Transparent && masterNode.backThenFrontRendering.isOn)
                {
                    pass.CullOverride = "Cull Back";
                }
                else
                {
                    pass.CullOverride = null;
                }
            }
        };

        private static HashSet<string> GetActiveFieldsFromMasterNode(INode iMasterNode, Pass pass)
        {
            HashSet<string> activeFields = new HashSet<string>();

            AIHDMasterNode masterNode = iMasterNode as AIHDMasterNode;
            if (masterNode == null)
            {
                return activeFields;
            }
			
			if (masterNode.doubleSidedMode != DoubleSidedMode.Disabled)
            {
                activeFields.Add("DoubleSided");
                if (pass.ShaderPassName != "SHADERPASS_VELOCITY")   // HACK to get around lack of a good interpolator dependency system
                {                                                   // we need to be able to build interpolators using multiple input structs
                                                                    // also: should only require isFrontFace if Normals are required...
                    if (masterNode.doubleSidedMode == DoubleSidedMode.FlippedNormals)
                    {
                        activeFields.Add("DoubleSided.Flip");
                    }
                    else if (masterNode.doubleSidedMode == DoubleSidedMode.MirroredNormals)
                    {
                        activeFields.Add("DoubleSided.Mirror");
                    }
                    // Important: the following is used in SharedCode.template.hlsl for determining the normal flip mode
                    activeFields.Add("FragInputs.isFrontFace");
                }
            }

            switch (masterNode.materialType)
            {
                case AIHDMasterNode.MaterialType.Anisotropy:
                    activeFields.Add("Material.Anisotropy");
                    break;
                case AIHDMasterNode.MaterialType.Iridescence:
                    activeFields.Add("Material.Iridescence");
                    break;
                case AIHDMasterNode.MaterialType.SpecularColor:
                    activeFields.Add("Material.SpecularColor");
                    break;
                case AIHDMasterNode.MaterialType.Standard:
                    activeFields.Add("Material.Standard");
                    break;
                case AIHDMasterNode.MaterialType.SubsurfaceScattering:
                    {
                        if (masterNode.surfaceType != SurfaceType.Transparent)
                        {
                            activeFields.Add("Material.SubsurfaceScattering");
                        }                        
                        if (masterNode.sssTransmission.isOn)
                        {
                            activeFields.Add("Material.Transmission");
                        }
                    }
                    break;
                case AIHDMasterNode.MaterialType.Translucent:
                    {
                        activeFields.Add("Material.Translucent");
                        activeFields.Add("Material.Transmission");
                    }
                    break;
                default:
                    UnityEngine.Debug.LogError("Unknown material type: " + masterNode.materialType);
                    break;
            }
			if( pass.PixelShaderUsesSlot( AIHDMasterNode.AlphaThresholdSlotId ) )
			{
				activeFields.Add( "AlphaTest" );
			}

			if( masterNode.surfaceType != SurfaceType.Opaque)
            {
                activeFields.Add("SurfaceType.Transparent");

                if (masterNode.alphaMode == AlphaMode.Alpha)
                {
                    activeFields.Add("BlendMode.Alpha");
                }
                else if (masterNode.alphaMode == AlphaMode.Premultiply)
                {
                    activeFields.Add("BlendMode.Premultiply");
                }
                else if (masterNode.alphaMode == AlphaMode.Additive)
                {
                    activeFields.Add("BlendMode.Add");
                }

                if (masterNode.blendPreserveSpecular.isOn)
                {
                    activeFields.Add("BlendMode.PreserveSpecular");
                }

                if (masterNode.transparencyFog.isOn)
                {
                    activeFields.Add("AlphaFog");
                }
            }

            if (!masterNode.receiveDecals.isOn)
            {
                activeFields.Add("DisableDecals");
            }

            if (!masterNode.receiveSSR.isOn)
            {
                activeFields.Add("DisableSSR");
            }


            if (masterNode.specularAA.isOn && pass.PixelShaderUsesSlot(AIHDMasterNode.SpecularAAThresholdSlotId) && pass.PixelShaderUsesSlot(AIHDMasterNode.SpecularAAScreenSpaceVarianceSlotId))
            {
                activeFields.Add("Specular.AA");
            }

            if (masterNode.energyConservingSpecular.isOn)
            {
                activeFields.Add("Specular.EnergyConserving");
            }

            if (masterNode.HasRefraction())
            {
                activeFields.Add("Refraction");
                switch (masterNode.refractionModel)
                {
                    case ScreenSpaceRefraction.RefractionModel.Box:
                        activeFields.Add("RefractionBox");
                        break;

                    case ScreenSpaceRefraction.RefractionModel.Sphere:
                        activeFields.Add("RefractionSphere");
                        break;

                    default:
                        UnityEngine.Debug.LogError("Unknown refraction model: " + masterNode.refractionModel);
                        break;
                }
            }

            if (masterNode.IsSlotConnected(AIHDMasterNode.BentNormalSlotId) && pass.PixelShaderUsesSlot(AIHDMasterNode.BentNormalSlotId))
            {
                activeFields.Add("BentNormal");
            }

            if (masterNode.IsSlotConnected(AIHDMasterNode.TangentSlotId) && pass.PixelShaderUsesSlot(AIHDMasterNode.TangentSlotId))
            {
                activeFields.Add("Tangent");
            }

            switch (masterNode.specularOcclusionMode)
            {
                case SpecularOcclusionMode.Off:
                    break;
                case SpecularOcclusionMode.FromAO:
                    activeFields.Add("SpecularOcclusionFromAO");
                    break;
                case SpecularOcclusionMode.FromAOAndBentNormal:
                    activeFields.Add("SpecularOcclusionFromAOBentNormal");
                    break;
                case SpecularOcclusionMode.Custom:
                    activeFields.Add("SpecularOcclusionCustom");
                    break;

                default:
                    break;
            }

            if (pass.PixelShaderUsesSlot(AIHDMasterNode.AmbientOcclusionSlotId))
            {
                var occlusionSlot = masterNode.FindSlot<Vector1MaterialSlot>(AIHDMasterNode.AmbientOcclusionSlotId);

                bool connected = masterNode.IsSlotConnected(AIHDMasterNode.AmbientOcclusionSlotId);
                if (connected || occlusionSlot.value != occlusionSlot.defaultValue)
                {
                    activeFields.Add("AmbientOcclusion");
                }
            }

            if (pass.PixelShaderUsesSlot(AIHDMasterNode.CoatMaskSlotId))
            {
                var coatMaskSlot = masterNode.FindSlot<Vector1MaterialSlot>(AIHDMasterNode.CoatMaskSlotId);

                bool connected = masterNode.IsSlotConnected(AIHDMasterNode.CoatMaskSlotId);
                if (connected || coatMaskSlot.value > 0.0f)
                {
                    activeFields.Add("CoatMask");
                }
            }

			if( masterNode.IsSlotConnected( AIHDMasterNode.OutputRT0SlotId ) && pass.PixelShaderUsesSlot( AIHDMasterNode.OutputRT0SlotId ) )
			{
				var out0Slot = masterNode.FindSlot<Vector4MaterialSlot>( AIHDMasterNode.OutputRT0SlotId );

				bool connected = masterNode.IsSlotConnected( AIHDMasterNode.OutputRT0SlotId );
				if( connected || out0Slot.value != out0Slot.defaultValue )
				{
					activeFields.Add( "OutputRT0" );
				}
			}

			if( masterNode.IsSlotConnected( AIHDMasterNode.OutputRT1SlotId ) && pass.PixelShaderUsesSlot( AIHDMasterNode.OutputRT1SlotId ) )
			{
				var out1Slot = masterNode.FindSlot<Vector4MaterialSlot>( AIHDMasterNode.OutputRT1SlotId );

				bool connected = masterNode.IsSlotConnected( AIHDMasterNode.OutputRT1SlotId );
				if( connected || out1Slot.value != out1Slot.defaultValue )
				{
					activeFields.Add( "OutputRT1" );
				}
			}

			if( masterNode.IsSlotConnected( AIHDMasterNode.OutputRT2SlotId ) && pass.PixelShaderUsesSlot( AIHDMasterNode.OutputRT2SlotId ) )
			{
				var out2Slot = masterNode.FindSlot<Vector4MaterialSlot>( AIHDMasterNode.OutputRT2SlotId );

				bool connected = masterNode.IsSlotConnected( AIHDMasterNode.OutputRT2SlotId );
				if( connected || out2Slot.value != out2Slot.defaultValue )
				{
					activeFields.Add( "OutputRT2" );
				}
			}

			if( masterNode.IsSlotConnected( AIHDMasterNode.OutputRT3SlotId ) && pass.PixelShaderUsesSlot( AIHDMasterNode.OutputRT3SlotId ) )
			{
				var out3Slot = masterNode.FindSlot<Vector4MaterialSlot>( AIHDMasterNode.OutputRT3SlotId );

				bool connected = masterNode.IsSlotConnected( AIHDMasterNode.OutputRT3SlotId );
				if( connected || out3Slot.value != out3Slot.defaultValue )
				{
					activeFields.Add( "OutputRT3" );
				}
			}

			if( masterNode.IsSlotConnected( AIHDMasterNode.OutputRT4SlotId ) && pass.PixelShaderUsesSlot( AIHDMasterNode.OutputRT4SlotId ) )
			{
				var out4Slot = masterNode.FindSlot<Vector4MaterialSlot>( AIHDMasterNode.OutputRT4SlotId );

				bool connected = masterNode.IsSlotConnected( AIHDMasterNode.OutputRT4SlotId );
				if( connected || out4Slot.value != out4Slot.defaultValue )
				{
					activeFields.Add( "OutputRT4" );
				}
			}

			if( masterNode.IsSlotConnected( AIHDMasterNode.OutputRT5SlotId ) && pass.PixelShaderUsesSlot( AIHDMasterNode.OutputRT5SlotId ) )
			{
				var out5Slot = masterNode.FindSlot<Vector4MaterialSlot>( AIHDMasterNode.OutputRT5SlotId );

				bool connected = masterNode.IsSlotConnected( AIHDMasterNode.OutputRT5SlotId );
				if( connected || out5Slot.value != out5Slot.defaultValue )
				{
					activeFields.Add( "OutputRT5" );
				}
			}

			if( masterNode.IsSlotConnected( AIHDMasterNode.OutputRT6SlotId ) && pass.PixelShaderUsesSlot( AIHDMasterNode.OutputRT6SlotId ) )
			{
				var out6Slot = masterNode.FindSlot<Vector4MaterialSlot>( AIHDMasterNode.OutputRT6SlotId );

				bool connected = masterNode.IsSlotConnected( AIHDMasterNode.OutputRT6SlotId );
				if( connected || out6Slot.value != out6Slot.defaultValue )
				{
					activeFields.Add( "OutputRT6" );
				}
			}

			if( masterNode.IsSlotConnected( AIHDMasterNode.OutputRT7SlotId ) && pass.PixelShaderUsesSlot( AIHDMasterNode.OutputRT7SlotId ) )
			{
				var out7Slot = masterNode.FindSlot<Vector4MaterialSlot>( AIHDMasterNode.OutputRT7SlotId );

				bool connected = masterNode.IsSlotConnected( AIHDMasterNode.OutputRT7SlotId );
				if( connected || out7Slot.value != out7Slot.defaultValue )
				{
					activeFields.Add( "OutputRT7" );
				}
			}

			return activeFields;
        }

        private static bool GenerateShaderPassLit(AIHDMasterNode masterNode, Pass pass, GenerationMode mode, ShaderGenerator result, List<string> sourceAssetDependencyPaths)
        {
            if (mode == GenerationMode.ForReals || pass.UseInPreview)
            {
                SurfaceMaterialOptions materialOptions = HDSubShaderUtilities.BuildMaterialOptions(masterNode.surfaceType, masterNode.alphaMode, masterNode.doubleSidedMode != DoubleSidedMode.Disabled, masterNode.HasRefraction());

                pass.OnGeneratePass(masterNode);

                // apply master node options to active fields
                HashSet<string> activeFields = GetActiveFieldsFromMasterNode(masterNode, pass);

                // use standard shader pass generation
                bool vertexActive = masterNode.IsSlotConnected(AIHDMasterNode.PositionSlotId);
                return GenerateShaderPass(masterNode, pass, mode, materialOptions, activeFields, result, sourceAssetDependencyPaths, vertexActive);
            }
            else
            {
                return false;
            }
        }

        public string GetSubshader(IMasterNode iMasterNode, GenerationMode mode, List<string> sourceAssetDependencyPaths = null)
        {
            if (sourceAssetDependencyPaths != null)
            {
				// AIHDSubShader.cs
				sourceAssetDependencyPaths.Add(AssetDatabase.GUIDToAssetPath( "04a950c6fabc95a42966e4aa8f02962e" ) );
                // HDSubShaderUtilities.cs
                sourceAssetDependencyPaths.Add(AssetDatabase.GUIDToAssetPath("713ced4e6eef4a44799a4dd59041484b"));
            }

            var masterNode = iMasterNode as AIHDMasterNode;

            var subShader = new ShaderGenerator();
            subShader.AddShaderChunk("SubShader", true);
            subShader.AddShaderChunk("{", true);
            subShader.Indent();
            {
                SurfaceMaterialTags materialTags = HDSubShaderUtilities.BuildMaterialTags(masterNode.surfaceType, masterNode.alphaTest.isOn, masterNode.drawBeforeRefraction.isOn, masterNode.sortPriority);

                // Add tags at the SubShader level
                {
                    var tagsVisitor = new ShaderStringBuilder();
                    materialTags.GetTags(tagsVisitor, HDRenderPipeline.k_ShaderTagName);
                    subShader.AddShaderChunk(tagsVisitor.ToString(), false);
                }

                // generate the necessary shader passes
                bool opaque = (masterNode.surfaceType == SurfaceType.Opaque);
                bool transparent = !opaque;

                bool distortionActive = transparent && masterNode.distortion.isOn;
                bool transparentBackfaceActive = transparent && masterNode.backThenFrontRendering.isOn;
                bool transparentDepthPrepassActive = transparent && masterNode.alphaTest.isOn && masterNode.alphaTestDepthPrepass.isOn;
                bool transparentDepthPostpassActive = transparent && masterNode.alphaTest.isOn && masterNode.alphaTestDepthPostpass.isOn;

                // Assign define here based on opaque or transparent to save some variant
                m_PassForward.ExtraDefines = opaque ? HDSubShaderUtilities.s_ExtraDefinesForwardOpaque : HDSubShaderUtilities.s_ExtraDefinesForwardTransparent;
                GenerateShaderPassLit(masterNode, m_PassForward, mode, subShader, sourceAssetDependencyPaths);
            }
            subShader.Deindent();
            subShader.AddShaderChunk("}", true);

            subShader.AddShaderChunk(@"CustomEditor ""UnityEditor.ShaderGraph.HDLitGUI""");

            return subShader.GetShaderString(0);
        }

        public bool IsPipelineCompatible(RenderPipelineAsset renderPipelineAsset)
        {
            return renderPipelineAsset is HDRenderPipelineAsset;
        }

		public static class HDRPShaderStructsEx
		{
			private static System.Type type = null;
			public static System.Type Type { get { return ( type == null ) ? type = System.Type.GetType( "UnityEditor.Experimental.Rendering.HDPipeline.HDRPShaderStructs, Unity.RenderPipelines.HighDefinition.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" ) : type; } }

			public static void AddActiveFieldsFromPixelGraphRequirements( HashSet<string> activeFields, ShaderGraphRequirements requirements )
			{
				object[] parameters = new object[] { activeFields, requirements };
				MethodInfo method = Type.GetMethod( "AddActiveFieldsFromPixelGraphRequirements", BindingFlags.Static | BindingFlags.Public );
				method.Invoke( null, parameters );
			}

			public static void AddActiveFieldsFromVertexGraphRequirements( HashSet<string> activeFields, ShaderGraphRequirements requirements )
			{
				object[] parameters = new object[] { activeFields, requirements };
				MethodInfo method = Type.GetMethod( "AddActiveFieldsFromVertexGraphRequirements", BindingFlags.Static | BindingFlags.Public );
				method.Invoke( null, parameters );
			}

			public static void AddRequiredFields( List<string> passRequiredFields, HashSet<string> activeFields )
			{
				object[] parameters = new object[] { passRequiredFields, activeFields };
				MethodInfo method = Type.GetMethod( "AddRequiredFields", BindingFlags.Static | BindingFlags.Public );
				method.Invoke( null, parameters );
			}

			public static Dependency[] FragInputsDependencies
			{
				get
				{
					var field = Type.GetNestedType( "FragInputs", BindingFlags.NonPublic ).GetField( "dependencies" );
					return field.GetValue( null ) as Dependency[];
				}
			}

			public static Dependency[] VaryingsMeshToPSStandardDependencies
			{
				get
				{
					var field = Type.GetNestedType( "VaryingsMeshToPS", BindingFlags.NonPublic ).GetField( "standardDependencies" );
					return field.GetValue( null ) as Dependency[];
				}
			}

			public static Dependency[] SurfaceDescriptionInputsDependencies
			{
				get
				{
					var field = Type.GetNestedType( "SurfaceDescriptionInputs", BindingFlags.NonPublic ).GetField( "dependencies" );
					return field.GetValue( null ) as Dependency[];
				}
			}

			public static Dependency[] VertexDescriptionInputsDependencies
			{
				get
				{
					var field = Type.GetNestedType( "VertexDescriptionInputs", BindingFlags.NonPublic ).GetField( "dependencies" );
					return field.GetValue( null ) as Dependency[];
				}
			}

			public static System.Type SurfaceDescriptionInputs
			{
				get
				{
					return Type.GetNestedType( "SurfaceDescriptionInputs", BindingFlags.NonPublic );
				}
			}

			public static System.Type VertexDescriptionInputs
			{
				get
				{
					return Type.GetNestedType( "VertexDescriptionInputs", BindingFlags.NonPublic );
				}
			}
		}

		public static bool GenerateShaderPass( AbstractMaterialNode masterNode, Pass pass, GenerationMode mode, SurfaceMaterialOptions materialOptions, HashSet<string> activeFields, ShaderGenerator result, List<string> sourceAssetDependencyPaths, bool vertexActive )
		{
			string templatePath = Path.Combine( HDUtils.GetHDRenderPipelinePath(), "Editor/Material" );
			string templateLocation = Path.Combine( Path.Combine( Path.Combine( templatePath, pass.MaterialName ), "ShaderGraph" ), pass.TemplateName );
			//Debug.Log( templatePath );
			//Debug.Log( templateLocation );
			templateLocation = AssetDatabase.GUIDToAssetPath( "a2f0392a03893724593899cd98465f63" );
			if( !File.Exists( templateLocation ) )
			{
				// TODO: produce error here
				Debug.LogError( "Template not found: " + templateLocation );
				return false;
			}

			bool debugOutput = false;

			// grab all of the active nodes (for pixel and vertex graphs)
			var vertexNodes = ListPool<INode>.Get();
			NodeUtils.DepthFirstCollectNodesFromNode( vertexNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.VertexShaderSlots );

			var pixelNodes = ListPool<INode>.Get();
			NodeUtils.DepthFirstCollectNodesFromNode( pixelNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.PixelShaderSlots );

			// graph requirements describe what the graph itself requires
			var pixelRequirements = ShaderGraphRequirements.FromNodes( pixelNodes, ShaderStageCapability.Fragment, false );   // TODO: is ShaderStageCapability.Fragment correct?
			var vertexRequirements = ShaderGraphRequirements.FromNodes( vertexNodes, ShaderStageCapability.Vertex, false );
			var graphRequirements = pixelRequirements.Union( vertexRequirements );

			// Function Registry tracks functions to remove duplicates, it wraps a string builder that stores the combined function string
			ShaderStringBuilder graphNodeFunctions = new ShaderStringBuilder();
			graphNodeFunctions.IncreaseIndent();
			var functionRegistry = new FunctionRegistry( graphNodeFunctions );

			// TODO: this can be a shared function for all HDRP master nodes -- From here through GraphUtil.GenerateSurfaceDescription(..)

			// Build the list of active slots based on what the pass requires
			var pixelSlots = HDSubShaderUtilities.FindMaterialSlotsOnNode( pass.PixelShaderSlots, masterNode );
			var vertexSlots = HDSubShaderUtilities.FindMaterialSlotsOnNode( pass.VertexShaderSlots, masterNode );

			// properties used by either pixel and vertex shader
			PropertyCollector sharedProperties = new PropertyCollector();

			// build the graph outputs structure to hold the results of each active slots (and fill out activeFields to indicate they are active)
			string pixelGraphInputStructName = "SurfaceDescriptionInputs";
			string pixelGraphOutputStructName = "SurfaceDescription";
			string pixelGraphEvalFunctionName = "SurfaceDescriptionFunction";
			ShaderStringBuilder pixelGraphEvalFunction = new ShaderStringBuilder();
			ShaderStringBuilder pixelGraphOutputs = new ShaderStringBuilder();

			// build initial requirements
			HDRPShaderStructsEx.AddActiveFieldsFromPixelGraphRequirements( activeFields, pixelRequirements );

			// build the graph outputs structure, and populate activeFields with the fields of that structure
			GraphUtil.GenerateSurfaceDescriptionStruct( pixelGraphOutputs, pixelSlots, true, pixelGraphOutputStructName, activeFields );

			// Build the graph evaluation code, to evaluate the specified slots
			GraphUtil.GenerateSurfaceDescriptionFunction(
				pixelNodes,
				masterNode,
				masterNode.owner as AbstractMaterialGraph,
				pixelGraphEvalFunction,
				functionRegistry,
				sharedProperties,
				pixelRequirements,  // TODO : REMOVE UNUSED
				mode,
				pixelGraphEvalFunctionName,
				pixelGraphOutputStructName,
				null,
				pixelSlots,
				pixelGraphInputStructName );

			string vertexGraphInputStructName = "VertexDescriptionInputs";
			string vertexGraphOutputStructName = "VertexDescription";
			string vertexGraphEvalFunctionName = "VertexDescriptionFunction";
			ShaderStringBuilder vertexGraphEvalFunction = new ShaderStringBuilder();
			ShaderStringBuilder vertexGraphOutputs = new ShaderStringBuilder();

			// check for vertex animation -- enables HAVE_VERTEX_MODIFICATION
			if( vertexActive )
			{
				vertexActive = true;
				activeFields.Add( "features.modifyMesh" );
				HDRPShaderStructsEx.AddActiveFieldsFromVertexGraphRequirements( activeFields, vertexRequirements );

				// -------------------------------------
				// Generate Output structure for Vertex Description function
				GraphUtil.GenerateVertexDescriptionStruct( vertexGraphOutputs, vertexSlots, vertexGraphOutputStructName, activeFields );

				// -------------------------------------
				// Generate Vertex Description function
				GraphUtil.GenerateVertexDescriptionFunction(
					masterNode.owner as AbstractMaterialGraph,
					vertexGraphEvalFunction,
					functionRegistry,
					sharedProperties,
					mode,
					vertexNodes,
					vertexSlots,
					vertexGraphInputStructName,
					vertexGraphEvalFunctionName,
					vertexGraphOutputStructName );
			}

			var blendCode = new ShaderStringBuilder();
			var cullCode = new ShaderStringBuilder();
			var zTestCode = new ShaderStringBuilder();
			var zWriteCode = new ShaderStringBuilder();
			var zClipCode = new ShaderStringBuilder();
			var stencilCode = new ShaderStringBuilder();
			var colorMaskCode = new ShaderStringBuilder();
			HDSubShaderUtilities.BuildRenderStatesFromPassAndMaterialOptions( pass, materialOptions, blendCode, cullCode, zTestCode, zWriteCode, zClipCode, stencilCode, colorMaskCode );

			HDRPShaderStructsEx.AddRequiredFields( pass.RequiredFields, activeFields );

			// propagate active field requirements using dependencies
			ShaderSpliceUtil.ApplyDependencies(
				activeFields,
				new List<Dependency[]>()
				{
					HDRPShaderStructsEx.FragInputsDependencies,
					HDRPShaderStructsEx.VaryingsMeshToPSStandardDependencies,
					HDRPShaderStructsEx.SurfaceDescriptionInputsDependencies,
					HDRPShaderStructsEx.VertexDescriptionInputsDependencies
				} );

			// debug output all active fields
			var interpolatorDefines = new ShaderGenerator();
			if( debugOutput )
			{
				interpolatorDefines.AddShaderChunk( "// ACTIVE FIELDS:" );
				foreach( string f in activeFields )
				{
					interpolatorDefines.AddShaderChunk( "//   " + f );
				}
			}

			// build graph inputs structures
			ShaderGenerator pixelGraphInputs = new ShaderGenerator();
			ShaderSpliceUtil.BuildType( HDRPShaderStructsEx.SurfaceDescriptionInputs, activeFields, pixelGraphInputs );
			ShaderGenerator vertexGraphInputs = new ShaderGenerator();
			ShaderSpliceUtil.BuildType( HDRPShaderStructsEx.VertexDescriptionInputs, activeFields, vertexGraphInputs );

			ShaderGenerator defines = new ShaderGenerator();
			{
				defines.AddShaderChunk( string.Format( "#define SHADERPASS {0}", pass.ShaderPassName ), true );
				if( pass.ExtraDefines != null )
				{
					foreach( var define in pass.ExtraDefines )
						defines.AddShaderChunk( define );
				}
				if( graphRequirements.requiresDepthTexture )
					defines.AddShaderChunk( "#define REQUIRE_DEPTH_TEXTURE" );
				if( graphRequirements.requiresCameraOpaqueTexture )
					defines.AddShaderChunk( "#define REQUIRE_OPAQUE_TEXTURE" );
				defines.AddGenerator( interpolatorDefines );
			}

			var shaderPassIncludes = new ShaderGenerator();
			if( pass.Includes != null )
			{
				foreach( var include in pass.Includes )
					shaderPassIncludes.AddShaderChunk( include );
			}


			// build graph code
			var graph = new ShaderGenerator();
			{
				graph.AddShaderChunk( "// Shared Graph Properties (uniform inputs)" );
				graph.AddShaderChunk( sharedProperties.GetPropertiesDeclaration( 1 ) );

				if( vertexActive )
				{
					graph.AddShaderChunk( "// Vertex Graph Inputs" );
					graph.Indent();
					graph.AddGenerator( vertexGraphInputs );
					graph.Deindent();
					graph.AddShaderChunk( "// Vertex Graph Outputs" );
					graph.Indent();
					graph.AddShaderChunk( vertexGraphOutputs.ToString() );
					graph.Deindent();
				}

				graph.AddShaderChunk( "// Pixel Graph Inputs" );
				graph.Indent();
				graph.AddGenerator( pixelGraphInputs );
				graph.Deindent();
				graph.AddShaderChunk( "// Pixel Graph Outputs" );
				graph.Indent();
				graph.AddShaderChunk( pixelGraphOutputs.ToString() );
				graph.Deindent();

				graph.AddShaderChunk( "// Shared Graph Node Functions" );
				graph.AddShaderChunk( graphNodeFunctions.ToString() );

				if( vertexActive )
				{
					graph.AddShaderChunk( "// Vertex Graph Evaluation" );
					graph.Indent();
					graph.AddShaderChunk( vertexGraphEvalFunction.ToString() );
					graph.Deindent();
				}

				graph.AddShaderChunk( "// Pixel Graph Evaluation" );
				graph.Indent();
				graph.AddShaderChunk( pixelGraphEvalFunction.ToString() );
				graph.Deindent();
			}

			// build the hash table of all named fragments      TODO: could make this Dictionary<string, ShaderGenerator / string>  ?
			Dictionary<string, string> namedFragments = new Dictionary<string, string>();
			namedFragments.Add( "Defines", defines.GetShaderString( 2, false ) );
			namedFragments.Add( "Graph", graph.GetShaderString( 2, false ) );
			namedFragments.Add( "LightMode", pass.LightMode );
			namedFragments.Add( "PassName", pass.Name );
			namedFragments.Add( "Includes", shaderPassIncludes.GetShaderString( 2, false ) );
			namedFragments.Add( "Blending", blendCode.ToString() );
			namedFragments.Add( "Culling", cullCode.ToString() );
			namedFragments.Add( "ZTest", zTestCode.ToString() );
			namedFragments.Add( "ZWrite", zWriteCode.ToString() );
			namedFragments.Add( "ZClip", zClipCode.ToString() );
			namedFragments.Add( "Stencil", stencilCode.ToString() );
			namedFragments.Add( "ColorMask", colorMaskCode.ToString() );
			namedFragments.Add( "LOD", materialOptions.lod.ToString() );

			// this is the format string for building the 'C# qualified assembly type names' for $buildType() commands
			string buildTypeAssemblyNameFormat = "UnityEditor.Experimental.Rendering.HDPipeline.HDRPShaderStructs+{0}, " + typeof( HDSubShaderUtilities ).Assembly.FullName.ToString();

			string sharedTemplatePath = Path.Combine( Path.Combine( HDUtils.GetHDRenderPipelinePath(), "Editor" ), "ShaderGraph" );
			// process the template to generate the shader code for this pass
			ShaderSpliceUtil.TemplatePreprocessor templatePreprocessor =
				new ShaderSpliceUtil.TemplatePreprocessor( activeFields, namedFragments, debugOutput, sharedTemplatePath, sourceAssetDependencyPaths, buildTypeAssemblyNameFormat );

			templatePreprocessor.ProcessTemplateFile( templateLocation );

			result.AddShaderChunk( templatePreprocessor.GetShaderCode().ToString(), false );

			return true;
		}
	}
}
#endif
