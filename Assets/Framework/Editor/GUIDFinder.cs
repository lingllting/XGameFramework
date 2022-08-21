using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace XGameFramework.EditorTools.AssetManagement
{
	public class GUIDFinder : EditorWindow
	{
		enum SearchState
		{
			None,
			SearchedGuid,
			SearchedHash,
			InvalidHashString
		}
		
		private string _guid;
		private string _assetPath;
		private string _hash;
		private SearchState _searchedState = SearchState.None;
		private Dictionary< int, string > _hashToPathDictionary = null;

		[MenuItem( "XGameFramework/AssetManagement/Find file using GUID or Hash ..." )]
		public static void OpenFind()
		{
			GUIDFinder window = EditorWindow.GetWindow<GUIDFinder>( true, "Find file" );
			window.minSize = new Vector2( 600, 90 );
			window.maxSize = new Vector2( int.MaxValue, 90 );
		}
		
		private void OnGUI()
		{
			EditorGUI.BeginChangeCheck();
			_guid = EditorGUILayout.TextField( "GUID", _guid );
			_hash = EditorGUILayout.TextField( "Hash", _hash );
			
			if ( EditorGUI.EndChangeCheck() )
			{
				_searchedState = SearchState.None;
				_assetPath = "";
			}
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			
			if ( GUILayout.Button( "Search GUID" ) )
			{
				_assetPath = AssetDatabase.GUIDToAssetPath( _guid );
				_searchedState = SearchState.SearchedGuid;
			}
			
			if ( GUILayout.Button( "Search Hash" ) )
			{
				int hashNumber;
				if ( Int32.TryParse(_hash, out hashNumber) )
				{
					if ( _hashToPathDictionary == null )
					{
						string[] paths = AssetDatabase.GetAllAssetPaths();
						_hashToPathDictionary = new Dictionary< int, string >();
				
						for ( int i = 0; i < paths.Length; i++ )
						{
							string path = paths[i];
						
							if ( i % 250 == 0 )
							{
								EditorUtility.DisplayProgressBar("Generating Hashes from GUIDs",
									string.Format("Analyzing {0}", path), ((float) (i + 1)) / (float) paths.Length);
							}
							_hashToPathDictionary[Animator.StringToHash(AssetDatabase.AssetPathToGUID(path))] = path;
						}
						EditorUtility.ClearProgressBar();
					}
					
					if (! _hashToPathDictionary.TryGetValue(hashNumber, out _assetPath) )
					{
						Debug.LogErrorFormat("Couldn't find Hash {0}", hashNumber);
					}
					_searchedState = SearchState.SearchedHash;

				}
				else
				{
					_searchedState = SearchState.InvalidHashString;
				}
			}

			GUI.enabled = _hashToPathDictionary != null;
			if ( GUILayout.Button("Clear Hash Dictionary") )
			{
				_hashToPathDictionary = null;
			}
			GUI.enabled = true;

			EditorGUILayout.EndHorizontal();
			
			if ( string.IsNullOrEmpty( _assetPath ) )
			{
				if ( _searchedState == SearchState.None )
				{
					EditorGUILayout.Space();
				}
				else if ( _searchedState == SearchState.SearchedGuid )
				{
					EditorGUILayout.HelpBox( string.Format( "Unable to find file with GUID {0}.", _guid ), MessageType.Error );
				}
				else if ( _searchedState == SearchState.SearchedHash )
				{
					EditorGUILayout.HelpBox( string.Format( "Unable to find file with Hash {0}.", _hash ), MessageType.Error );
				}
				else if ( _searchedState == SearchState.InvalidHashString )
				{
					EditorGUILayout.HelpBox( string.Format( "Couldn't parse {0} into an 32 bit Integer.", _hash ), MessageType.Error );
				}
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				
				
				EditorGUILayout.LabelField( _assetPath );
				
				if ( GUILayout.Button( "Select" ) )
				{
					Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>( _assetPath );
				}
				
				EditorGUILayout.EndHorizontal();
			}
			
		}
	}
}