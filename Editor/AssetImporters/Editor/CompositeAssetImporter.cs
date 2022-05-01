using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.AssetImporters;
using CompositeResult = PowerEditor.AssetImporters.CompositeAsset.CompositeResult;

namespace PowerEditor.AssetImporters
{
	[ScriptedImporter(0, ".comp")]
	public class CompositeAssetImporter : ScriptedImporter
	{
		public CompositeAsset compositeAsset;
		
		public override void OnImportAsset(AssetImportContext ctx)
		{
			string content = File.ReadAllText(ctx.assetPath);
			compositeAsset = CompositeAsset.Parse(content);

			ctx.DependsOnSourceAsset(ctx.assetPath);
			HashSet<string> dependentArtifacts = 
				new HashSet<string>(compositeAsset.dependentArtifacts
					.Where(path => !string.IsNullOrEmpty(path)));
			foreach (var dependentArtifact in dependentArtifacts)
			{
				ctx.DependsOnArtifact(dependentArtifact);
			}
			HashSet<string> dependentSourceAssets = 
				new HashSet<string>(compositeAsset.dependentSourceAssets
					.Where(path => !string.IsNullOrEmpty(path)));
			foreach (var dependentSourceAsset in dependentSourceAssets)
			{
				ctx.DependsOnSourceAsset(dependentSourceAsset);
			}

			Type type = compositeAsset.GetType();
			IEnumerable<CompositeResult> compositeResults = 
				type.GetMethod("Composite")
					.Invoke(compositeAsset, new object[] {this, ctx}) as IEnumerable<CompositeResult>;
			compositeAsset.compositeResults = compositeResults.ToArray();
			foreach (CompositeResult result in compositeAsset.compositeResults)
			{
				ctx.AddObjectToAsset(result.identifier, result.obj);
			}
			ctx.AddObjectToAsset("Composite Asset", compositeAsset);
			ctx.SetMainObject(compositeAsset);
		}
	}
}
