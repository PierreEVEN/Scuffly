#if !UNITY_2019_1_OR_NEWER
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    [Title("Master", "Amplify Impostor", "Bake HighDefinition" )]
    public class AIHDMasterNode : MasterNode<IAIHDSubShader>, IMayRequirePosition, IMayRequireNormal, IMayRequireTangent
    {
        public const string AlbedoSlotName = "Albedo";
        public const string AlbedoDisplaySlotName = "BaseColor";
        public const string NormalSlotName = "Normal";
        public const string BentNormalSlotName = "BentNormal";
        public const string TangentSlotName = "Tangent";
        public const string SubsurfaceMaskSlotName = "SubsurfaceMask";
        public const string ThicknessSlotName = "Thickness";
        public const string DiffusionProfileSlotName = "DiffusionProfile";
        public const string IridescenceMaskSlotName = "IridescenceMask";
        public const string IridescenceThicknessSlotName = "IridescenceThickness";
        public const string SpecularColorSlotName = "Specular";
        public const string SpecularColorDisplaySlotName = "SpecularColor";
        public const string CoatMaskSlotName = "CoatMask";
        public const string EmissionSlotName = "Emission";
        public const string MetallicSlotName = "Metallic";
        public const string SmoothnessSlotName = "Smoothness";
        public const string AmbientOcclusionSlotName = "Occlusion";
        public const string AmbientOcclusionDisplaySlotName = "AmbientOcclusion";
        public const string AlphaSlotName = "Alpha";
        public const string AlphaClipThresholdSlotName = "AlphaClipThreshold";
        public const string AlphaClipThresholdDepthPrepassSlotName = "AlphaClipThresholdDepthPrepass";
        public const string AlphaClipThresholdDepthPostpassSlotName = "AlphaClipThresholdDepthPostpass";
        public const string AnisotropySlotName = "Anisotropy";
        public const string PositionSlotName = "Position";
        public const string SpecularAAScreenSpaceVarianceSlotName = "SpecularAAScreenSpaceVariance";
        public const string SpecularAAThresholdSlotName = "SpecularAAThreshold";
        public const string RefractionIndexSlotName = "RefractionIndex";
        public const string RefractionColorSlotName = "RefractionColor";
        public const string RefractionDistanceSlotName = "RefractionDistance";
        public const string DistortionSlotName = "Distortion";
        public const string DistortionBlurSlotName = "DistortionBlur";
        public const string SpecularOcclusionSlotName = "SpecularOcclusion";
        public const string AlphaClipThresholdShadowSlotName = "AlphaClipThresholdShadow";
		public const string OutputRT0SlotName = "OutputRT0";
		public const string OutputRT1SlotName = "OutputRT1";
		public const string OutputRT2SlotName = "OutputRT2";
		public const string OutputRT3SlotName = "OutputRT3";
		public const string OutputRT4SlotName = "OutputRT4";
		public const string OutputRT5SlotName = "OutputRT5";
		public const string OutputRT6SlotName = "OutputRT6";
		public const string OutputRT7SlotName = "OutputRT7";

		public const int PositionSlotId = 0;
        public const int AlbedoSlotId = 1;
        public const int NormalSlotId = 2;
        public const int BentNormalSlotId = 3;
        public const int TangentSlotId = 4;
        public const int SubsurfaceMaskSlotId = 5;
        public const int ThicknessSlotId = 6;
        public const int DiffusionProfileSlotId = 7;
        public const int IridescenceMaskSlotId = 8;
        public const int IridescenceThicknessSlotId = 9;
        public const int SpecularColorSlotId = 10;
        public const int CoatMaskSlotId = 11;
        public const int MetallicSlotId = 12;
        public const int EmissionSlotId = 13;
        public const int SmoothnessSlotId = 14;
        public const int AmbientOcclusionSlotId = 15;
        public const int AlphaSlotId = 16;
        public const int AlphaThresholdSlotId = 17;
        public const int AlphaThresholdDepthPrepassSlotId = 18;
        public const int AlphaThresholdDepthPostpassSlotId = 19;
        public const int AnisotropySlotId = 20;
        public const int SpecularAAScreenSpaceVarianceSlotId = 21;
        public const int SpecularAAThresholdSlotId = 22;
        public const int RefractionIndexSlotId = 23;
        public const int RefractionColorSlotId = 24;
        public const int RefractionDistanceSlotId = 25;
        public const int DistortionSlotId = 26;
        public const int DistortionBlurSlotId = 27;
        public const int SpecularOcclusionSlotId = 28;
        public const int AlphaThresholdShadowSlotId = 29;
		public const int OutputRT0SlotId = 30;
		public const int OutputRT1SlotId = 31;
		public const int OutputRT2SlotId = 32;
		public const int OutputRT3SlotId = 33;
		public const int OutputRT4SlotId = 34;
		public const int OutputRT5SlotId = 35;
		public const int OutputRT6SlotId = 36;
		public const int OutputRT7SlotId = 37;

		public enum MaterialType
        {
            Standard,
            SubsurfaceScattering,
            Anisotropy,
            Iridescence,
            SpecularColor,
            Translucent
        }

        // Don't support Multiply
        public enum AlphaModeLit
        {
            Alpha,
            Premultiply,
            Additive,
        }

        // Just for convenience of doing simple masks. We could run out of bits of course.
        [Flags]
        enum SlotMask
        {
            None = 0,
            Position = 1 << PositionSlotId,
            Albedo = 1 << AlbedoSlotId,
            Normal = 1 << NormalSlotId,
            BentNormal = 1 << BentNormalSlotId,
            Tangent = 1 << TangentSlotId,
            SubsurfaceMask = 1 << SubsurfaceMaskSlotId,
            Thickness = 1 << ThicknessSlotId,
            DiffusionProfile = 1 << DiffusionProfileSlotId,
            IridescenceMask = 1 << IridescenceMaskSlotId,
            IridescenceLayerThickness = 1 << IridescenceThicknessSlotId,
            Specular = 1 << SpecularColorSlotId,
            CoatMask = 1 << CoatMaskSlotId,
            Metallic = 1 << MetallicSlotId,
            Emission = 1 << EmissionSlotId,
            Smoothness = 1 << SmoothnessSlotId,
            Occlusion = 1 << AmbientOcclusionSlotId,
            Alpha = 1 << AlphaSlotId,
            AlphaThreshold = 1 << AlphaThresholdSlotId,
            AlphaThresholdDepthPrepass = 1 << AlphaThresholdDepthPrepassSlotId,
            AlphaThresholdDepthPostpass = 1 << AlphaThresholdDepthPostpassSlotId,
            Anisotropy = 1 << AnisotropySlotId,
            SpecularOcclusion = 1 << SpecularOcclusionSlotId,
            AlphaThresholdShadow = 1 << AlphaThresholdShadowSlotId,
			OutputRT0 = 1 << OutputRT0SlotId,
			OutputRT1 = 1 << OutputRT1SlotId,
			OutputRT2 = 1 << OutputRT2SlotId,
			OutputRT3 = 1 << OutputRT3SlotId,
			OutputRT4 = 1 << OutputRT4SlotId,
			OutputRT5 = 1 << OutputRT5SlotId,
			OutputRT6 = 1 << OutputRT6SlotId,
			OutputRT7 = 1 << OutputRT7SlotId
		}

        const SlotMask StandardSlotMask = SlotMask.Position | SlotMask.Albedo | SlotMask.Normal | SlotMask.BentNormal | SlotMask.CoatMask | SlotMask.Emission | SlotMask.Metallic | SlotMask.Smoothness | SlotMask.Occlusion | SlotMask.SpecularOcclusion | SlotMask.Alpha | SlotMask.AlphaThreshold | SlotMask.AlphaThresholdDepthPrepass | SlotMask.AlphaThresholdDepthPostpass | SlotMask.AlphaThresholdShadow | SlotMask.OutputRT0 | SlotMask.OutputRT1 | SlotMask.OutputRT2 | SlotMask.OutputRT3 | SlotMask.OutputRT4 | SlotMask.OutputRT5 | SlotMask.OutputRT6 | SlotMask.OutputRT7;
        const SlotMask SubsurfaceScatteringSlotMask = SlotMask.Position | SlotMask.Albedo | SlotMask.Normal | SlotMask.BentNormal | SlotMask.SubsurfaceMask | SlotMask.Thickness | SlotMask.DiffusionProfile | SlotMask.CoatMask | SlotMask.Emission | SlotMask.Smoothness | SlotMask.Occlusion | SlotMask.SpecularOcclusion | SlotMask.Alpha | SlotMask.AlphaThreshold | SlotMask.AlphaThresholdDepthPrepass | SlotMask.AlphaThresholdDepthPostpass | SlotMask.AlphaThresholdShadow | SlotMask.OutputRT0 | SlotMask.OutputRT1 | SlotMask.OutputRT2 | SlotMask.OutputRT3 | SlotMask.OutputRT4 | SlotMask.OutputRT5 | SlotMask.OutputRT6 | SlotMask.OutputRT7;
        const SlotMask AnisotropySlotMask = SlotMask.Position | SlotMask.Albedo | SlotMask.Normal | SlotMask.BentNormal | SlotMask.Tangent | SlotMask.Anisotropy | SlotMask.CoatMask | SlotMask.Emission | SlotMask.Metallic | SlotMask.Smoothness | SlotMask.Occlusion | SlotMask.SpecularOcclusion | SlotMask.Alpha | SlotMask.AlphaThreshold | SlotMask.AlphaThresholdDepthPrepass | SlotMask.AlphaThresholdDepthPostpass | SlotMask.AlphaThresholdShadow | SlotMask.OutputRT0 | SlotMask.OutputRT1 | SlotMask.OutputRT2 | SlotMask.OutputRT3 | SlotMask.OutputRT4 | SlotMask.OutputRT5 | SlotMask.OutputRT6 | SlotMask.OutputRT7;
        const SlotMask IridescenceSlotMask = SlotMask.Position | SlotMask.Albedo | SlotMask.Normal | SlotMask.BentNormal | SlotMask.IridescenceMask | SlotMask.IridescenceLayerThickness | SlotMask.CoatMask | SlotMask.Emission | SlotMask.Metallic | SlotMask.Smoothness | SlotMask.Occlusion | SlotMask.SpecularOcclusion | SlotMask.Alpha | SlotMask.AlphaThreshold | SlotMask.AlphaThresholdDepthPrepass | SlotMask.AlphaThresholdDepthPostpass | SlotMask.AlphaThresholdShadow | SlotMask.OutputRT0 | SlotMask.OutputRT1 | SlotMask.OutputRT2 | SlotMask.OutputRT3 | SlotMask.OutputRT4 | SlotMask.OutputRT5 | SlotMask.OutputRT6 | SlotMask.OutputRT7;
        const SlotMask SpecularColorSlotMask = SlotMask.Position | SlotMask.Albedo | SlotMask.Normal | SlotMask.BentNormal | SlotMask.Specular | SlotMask.CoatMask | SlotMask.Emission | SlotMask.Smoothness | SlotMask.Occlusion | SlotMask.SpecularOcclusion | SlotMask.Alpha | SlotMask.AlphaThreshold | SlotMask.AlphaThresholdDepthPrepass | SlotMask.AlphaThresholdDepthPostpass | SlotMask.AlphaThresholdShadow | SlotMask.OutputRT0 | SlotMask.OutputRT1 | SlotMask.OutputRT2 | SlotMask.OutputRT3 | SlotMask.OutputRT4 | SlotMask.OutputRT5 | SlotMask.OutputRT6 | SlotMask.OutputRT7;
        const SlotMask TranslucentSlotMask = SlotMask.Position | SlotMask.Albedo | SlotMask.Normal | SlotMask.BentNormal | SlotMask.Thickness | SlotMask.DiffusionProfile | SlotMask.CoatMask | SlotMask.Emission | SlotMask.Smoothness | SlotMask.Occlusion | SlotMask.SpecularOcclusion | SlotMask.Alpha | SlotMask.AlphaThreshold | SlotMask.AlphaThresholdDepthPrepass | SlotMask.AlphaThresholdDepthPostpass | SlotMask.AlphaThresholdShadow | SlotMask.OutputRT0 | SlotMask.OutputRT1 | SlotMask.OutputRT2 | SlotMask.OutputRT3 | SlotMask.OutputRT4 | SlotMask.OutputRT5 | SlotMask.OutputRT6 | SlotMask.OutputRT7;

        // This could also be a simple array. For now, catch any mismatched data.
        SlotMask GetActiveSlotMask()
        {
            switch (materialType)
            {
                case MaterialType.Standard:
                    return StandardSlotMask;

                case MaterialType.SubsurfaceScattering:
                    return SubsurfaceScatteringSlotMask;

                case MaterialType.Anisotropy:
                    return AnisotropySlotMask;

                case MaterialType.Iridescence:
                    return IridescenceSlotMask;

                case MaterialType.SpecularColor:
                    return SpecularColorSlotMask;

                case MaterialType.Translucent:
                    return TranslucentSlotMask;

                default:
                    return SlotMask.None;
            }
        }

        bool MaterialTypeUsesSlotMask(SlotMask mask)
        {
            SlotMask activeMask = GetActiveSlotMask();
            return (activeMask & mask) != 0;
        }

        [SerializeField]
        SurfaceType m_SurfaceType;

        public SurfaceType surfaceType
        {
            get { return m_SurfaceType; }
            set
            {
                if (m_SurfaceType == value)
                    return;

                m_SurfaceType = value;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        AlphaMode m_AlphaMode;

        public AlphaMode alphaMode
        {
            get { return m_AlphaMode; }
            set
            {
                if (m_AlphaMode == value)
                    return;

                m_AlphaMode = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_BlendPreserveSpecular = true;

        public ToggleData blendPreserveSpecular
        {
            get { return new ToggleData(m_BlendPreserveSpecular); }
            set
            {
                if (m_BlendPreserveSpecular == value.isOn)
                    return;
                m_BlendPreserveSpecular = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_TransparencyFog = true;

        public ToggleData transparencyFog
        {
            get { return new ToggleData(m_TransparencyFog); }
            set
            {
                if (m_TransparencyFog == value.isOn)
                    return;
                m_TransparencyFog = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_DrawBeforeRefraction;

        public ToggleData drawBeforeRefraction
        {
            get { return new ToggleData(m_DrawBeforeRefraction); }
            set
            {
                if (m_DrawBeforeRefraction == value.isOn)
                    return;
                m_DrawBeforeRefraction = value.isOn;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        ScreenSpaceRefraction.RefractionModel m_RefractionModel;

        public ScreenSpaceRefraction.RefractionModel refractionModel
        {
            get { return m_RefractionModel; }
            set
            {
                if (m_RefractionModel == value)
                    return;

                m_RefractionModel = value;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        bool m_Distortion;

        public ToggleData distortion
        {
            get { return new ToggleData(m_Distortion); }
            set
            {
                if (m_Distortion == value.isOn)
                    return;
                m_Distortion = value.isOn;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        DistortionMode m_DistortionMode;

        public DistortionMode distortionMode
        {
            get { return m_DistortionMode; }
            set
            {
                if (m_DistortionMode == value)
                    return;

                m_DistortionMode = value;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        bool m_DistortionDepthTest = true;

        public ToggleData distortionDepthTest
        {
            get { return new ToggleData(m_DistortionDepthTest); }
            set
            {
                if (m_DistortionDepthTest == value.isOn)
                    return;
                m_DistortionDepthTest = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_AlphaTest;

        public ToggleData alphaTest
        {
            get { return new ToggleData(m_AlphaTest); }
            set
            {
                if (m_AlphaTest == value.isOn)
                    return;
                m_AlphaTest = value.isOn;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        bool m_AlphaTestDepthPrepass;

        public ToggleData alphaTestDepthPrepass
        {
            get { return new ToggleData(m_AlphaTestDepthPrepass); }
            set
            {
                if (m_AlphaTestDepthPrepass == value.isOn)
                    return;
                m_AlphaTestDepthPrepass = value.isOn;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        bool m_AlphaTestDepthPostpass;

        public ToggleData alphaTestDepthPostpass
        {
            get { return new ToggleData(m_AlphaTestDepthPostpass); }
            set
            {
                if (m_AlphaTestDepthPostpass == value.isOn)
                    return;
                m_AlphaTestDepthPostpass = value.isOn;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        bool m_AlphaTestShadow;

        public ToggleData alphaTestShadow
        {
            get { return new ToggleData(m_AlphaTestShadow); }
            set
            {
                if (m_AlphaTestShadow == value.isOn)
                    return;
                m_AlphaTestShadow = value.isOn;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        bool m_BackThenFrontRendering;

        public ToggleData backThenFrontRendering
        {
            get { return new ToggleData(m_BackThenFrontRendering); }
            set
            {
                if (m_BackThenFrontRendering == value.isOn)
                    return;
                m_BackThenFrontRendering = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        int m_SortPriority;

        public int sortPriority
        {
            get { return m_SortPriority; }
            set
            {
                if (m_SortPriority == value)
                    return;
                m_SortPriority = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        DoubleSidedMode m_DoubleSidedMode;

        public DoubleSidedMode doubleSidedMode
        {
            get { return m_DoubleSidedMode; }
            set
            {
                if (m_DoubleSidedMode == value)
                    return;

                m_DoubleSidedMode = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        MaterialType m_MaterialType;

        public MaterialType materialType
        {
            get { return m_MaterialType; }
            set
            {
                if (m_MaterialType == value)
                    return;

                m_MaterialType = value;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        bool m_SSSTransmission = true;

        public ToggleData sssTransmission
        {
            get { return new ToggleData(m_SSSTransmission); }
            set
            {
                m_SSSTransmission = value.isOn;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        bool m_ReceiveDecals = true;

        public ToggleData receiveDecals
        {
            get { return new ToggleData(m_ReceiveDecals); }
            set
            {
                if (m_ReceiveDecals == value.isOn)
                    return;
                m_ReceiveDecals = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_ReceivesSSR = true;
        public ToggleData receiveSSR
        {
            get { return new ToggleData(m_ReceivesSSR); }
            set
            {
                if (m_ReceivesSSR == value.isOn)
                    return;
                m_ReceivesSSR = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_EnergyConservingSpecular = true;

        public ToggleData energyConservingSpecular
        {
            get { return new ToggleData(m_EnergyConservingSpecular); }
            set
            {
                if (m_EnergyConservingSpecular == value.isOn)
                    return;
                m_EnergyConservingSpecular = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_SpecularAA;

        public ToggleData specularAA
        {
            get { return new ToggleData(m_SpecularAA); }
            set
            {
                if (m_SpecularAA == value.isOn)
                    return;
                m_SpecularAA = value.isOn;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        float m_SpecularAAScreenSpaceVariance;
    
        public float specularAAScreenSpaceVariance
        {
            get { return m_SpecularAAScreenSpaceVariance; }
            set
            {
                if (m_SpecularAAScreenSpaceVariance == value)
                    return;
                m_SpecularAAScreenSpaceVariance = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        float m_SpecularAAThreshold;

        public float specularAAThreshold
        {
            get { return m_SpecularAAThreshold; }
            set
            {
                if (m_SpecularAAThreshold == value)
                    return;
                m_SpecularAAThreshold = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        SpecularOcclusionMode m_SpecularOcclusionMode;

        public SpecularOcclusionMode specularOcclusionMode
        {
            get { return m_SpecularOcclusionMode; }
            set
            {
                if (m_SpecularOcclusionMode == value)
                    return;

                m_SpecularOcclusionMode = value;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        int m_DiffusionProfile;

        public int diffusionProfile
        {
            get { return m_DiffusionProfile; }
            set
            {
                if (m_DiffusionProfile == value)
                    return;

                m_DiffusionProfile = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public AIHDMasterNode()
        {
            UpdateNodeAfterDeserialization();
        }

        public bool HasRefraction()
        {
            return (surfaceType == SurfaceType.Transparent && !drawBeforeRefraction.isOn && refractionModel != ScreenSpaceRefraction.RefractionModel.None);
        }

        public bool HasDistortion()
        {
            return (surfaceType == SurfaceType.Transparent && distortion.isOn);
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            base.UpdateNodeAfterDeserialization();
			name = "Amplify Impostor HD";

			List<int> validSlots = new List<int>();
            if (MaterialTypeUsesSlotMask(SlotMask.Position))
            {
                AddSlot(new PositionMaterialSlot(PositionSlotId, PositionSlotName, PositionSlotName, CoordinateSpace.Object, ShaderStageCapability.Vertex));
                validSlots.Add(PositionSlotId);
            }
			
			if( MaterialTypeUsesSlotMask( SlotMask.Alpha ) )
			{
				AddSlot( new Vector1MaterialSlot( AlphaSlotId, AlphaSlotName, AlphaSlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment, hidden: true ) );
				validSlots.Add( AlphaSlotId );
			}
			if( MaterialTypeUsesSlotMask( SlotMask.AlphaThreshold ) )
			{
				AddSlot( new Vector1MaterialSlot( AlphaThresholdSlotId, "Clip", AlphaClipThresholdSlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment ) );
				validSlots.Add( AlphaThresholdSlotId );
			}
			if( MaterialTypeUsesSlotMask( SlotMask.OutputRT0 ) )
			{
				AddSlot( new Vector4MaterialSlot( OutputRT0SlotId, OutputRT0SlotName, OutputRT0SlotName, SlotType.Input, new Vector4( 0, 0, 0, 0 ), ShaderStageCapability.Fragment ) );
				validSlots.Add( OutputRT0SlotId );
			}
			if( MaterialTypeUsesSlotMask( SlotMask.OutputRT1 ) )
			{
				AddSlot( new Vector4MaterialSlot( OutputRT1SlotId, OutputRT1SlotName, OutputRT1SlotName, SlotType.Input, new Vector4( 0, 0, 0, 0 ), ShaderStageCapability.Fragment ) );
				validSlots.Add( OutputRT1SlotId );
			}
			if( MaterialTypeUsesSlotMask( SlotMask.OutputRT2 ) )
			{
				AddSlot( new Vector4MaterialSlot( OutputRT2SlotId, OutputRT2SlotName, OutputRT2SlotName, SlotType.Input, new Vector4( 0, 0, 0, 0 ), ShaderStageCapability.Fragment ) );
				validSlots.Add( OutputRT2SlotId );
			}
			if( MaterialTypeUsesSlotMask( SlotMask.OutputRT3 ) )
			{
				AddSlot( new Vector4MaterialSlot( OutputRT3SlotId, OutputRT3SlotName, OutputRT3SlotName, SlotType.Input, new Vector4( 0, 0, 0, 0 ), ShaderStageCapability.Fragment ) );
				validSlots.Add( OutputRT3SlotId );
			}
			if( MaterialTypeUsesSlotMask( SlotMask.OutputRT4 ) )
			{
				AddSlot( new Vector4MaterialSlot( OutputRT4SlotId, OutputRT4SlotName, OutputRT4SlotName, SlotType.Input, new Vector4( 0, 0, 0, 0 ), ShaderStageCapability.Fragment ) );
				validSlots.Add( OutputRT4SlotId );
			}
			if( MaterialTypeUsesSlotMask( SlotMask.OutputRT5 ) )
			{
				AddSlot( new Vector4MaterialSlot( OutputRT5SlotId, OutputRT5SlotName, OutputRT5SlotName, SlotType.Input, new Vector4( 0, 0, 0, 0 ), ShaderStageCapability.Fragment ) );
				validSlots.Add( OutputRT5SlotId );
			}
			if( MaterialTypeUsesSlotMask( SlotMask.OutputRT6 ) )
			{
				AddSlot( new Vector4MaterialSlot( OutputRT6SlotId, OutputRT6SlotName, OutputRT6SlotName, SlotType.Input, new Vector4( 0, 0, 0, 0 ), ShaderStageCapability.Fragment ) );
				validSlots.Add( OutputRT6SlotId );
			}
			if( MaterialTypeUsesSlotMask( SlotMask.OutputRT7 ) )
			{
				AddSlot( new Vector4MaterialSlot( OutputRT7SlotId, OutputRT7SlotName, OutputRT7SlotName, SlotType.Input, new Vector4( 0, 0, 0, 0 ), ShaderStageCapability.Fragment ) );
				validSlots.Add( OutputRT7SlotId );
			}

			RemoveSlotsNameNotMatching(validSlots, true);
        }

        protected override VisualElement CreateCommonSettingsElement()
        {
            return new AIHDSettingsView(this);
        }

        public NeededCoordinateSpace RequiresNormal(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequireNormal>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresNormal(stageCapability));
        }

        public NeededCoordinateSpace RequiresTangent(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequireTangent>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresTangent(stageCapability));
        }

        public NeededCoordinateSpace RequiresPosition(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequirePosition>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresPosition(stageCapability));
        }

        public bool RequiresSplitLighting()
        {
            return materialType == AIHDMasterNode.MaterialType.SubsurfaceScattering;
        }

        public override void CollectShaderProperties(PropertyCollector collector, GenerationMode generationMode)
        {
            // Trunk currently relies on checking material property "_EmissionColor" to allow emissive GI. If it doesn't find that property, or it is black, GI is forced off.
            // ShaderGraph doesn't use this property, so currently it inserts a dummy color (white). This dummy color may be removed entirely once the following PR has been merged in trunk: Pull request #74105
            // The user will then need to explicitly disable emissive GI if it is not needed.
            // To be able to automatically disable emission based on the ShaderGraph config when emission is black,
            // we will need a more general way to communicate this to the engine (not directly tied to a material property).
            collector.AddShaderProperty(new ColorShaderProperty()
            {
                overrideReferenceName = "_EmissionColor",
                hidden = true,
                value = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            });

            base.CollectShaderProperties(collector, generationMode);
        }
    }
}
#endif
