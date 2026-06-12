using UnityEngine;

namespace SilksongIngameSkinselector;

public sealed class HornetSkinSelectorBehaviour : MonoBehaviour
{
    private const string SkinsFolderName = "HornetSkins";
    private const string ActiveFolderName = "_active";
    private const string SelectionFileName = "selected_skin.txt";
    private static readonly string[] SpriteSheetNames = ["hornet.png", "spritesheet.png"];
    private readonly List<SkinOption> _skins = [];
    private Rect _windowRect = new(20f, 20f, 360f, 320f);
    private bool _showMenu;
    private int _selectedSkinIndex = -1;
    private string _status = "No skin applied.";

    public string SkinsRootPath { get; private set; } = string.Empty;
    public string ActiveSpriteSheetPath { get; private set; } = string.Empty;
    public string? SelectedSkinName => _selectedSkinIndex >= 0 && _selectedSkinIndex < _skins.Count ? _skins[_selectedSkinIndex].Name : null;

    private void Awake()
    {
        SkinsRootPath = Path.Combine(Application.persistentDataPath, SkinsFolderName);
        ActiveSpriteSheetPath = Path.Combine(SkinsRootPath, ActiveFolderName, "hornet.png");
        Directory.CreateDirectory(SkinsRootPath);
        Directory.CreateDirectory(Path.GetDirectoryName(ActiveSpriteSheetPath)!);
        ReloadSkins();
        LoadSavedSelection();
    }

    private void OnGUI()
    {
        Event currentEvent = Event.current;
        if (currentEvent.type == EventType.KeyDown)
        {
            if (currentEvent.keyCode == KeyCode.F8)
            {
                _showMenu = !_showMenu;
                currentEvent.Use();
            }
            else if (currentEvent.keyCode == KeyCode.F5)
            {
                ReloadSkins();
                currentEvent.Use();
            }
        }

        if (!_showMenu)
        {
            return;
        }

        _windowRect = GUI.Window(GetInstanceID(), _windowRect, DrawWindow, "Hornet Skin Selector");
    }

    private void DrawWindow(int id)
    {
        GUILayout.BeginVertical();
        GUILayout.Label("F8: open/close menu  |  F5: reload skins");
        GUILayout.Space(6f);

        if (_skins.Count == 0)
        {
            GUILayout.Label($"No skins found. Add folders in:\n{SkinsRootPath}");
        }
        else
        {
            GUILayout.Label("Available skins:");
            foreach ((SkinOption skin, int index) in _skins.Select((value, i) => (value, i)))
            {
                bool isCurrentSelection = index == _selectedSkinIndex;
                string buttonText = isCurrentSelection ? $"● {skin.Name}" : skin.Name;
                if (GUILayout.Button(buttonText))
                {
                    _selectedSkinIndex = index;
                }
            }

            GUILayout.Space(8f);
            if (GUILayout.Button("Apply selected skin"))
            {
                ApplySelectedSkin();
            }
        }

        GUILayout.Space(8f);
        if (GUILayout.Button("Reload skin folders"))
        {
            ReloadSkins();
        }

        GUILayout.Space(4f);
        GUILayout.Label(_status);
        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    private void ReloadSkins()
    {
        string? previousSelection = SelectedSkinName;
        _skins.Clear();

        foreach (string skinDirectory in Directory.EnumerateDirectories(SkinsRootPath))
        {
            string folderName = Path.GetFileName(skinDirectory);
            if (string.Equals(folderName, ActiveFolderName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string? spriteSheetPath = FindSpriteSheet(skinDirectory);
            if (spriteSheetPath is not null)
            {
                _skins.Add(new SkinOption(folderName, spriteSheetPath));
            }
        }

        _selectedSkinIndex = _skins.FindIndex(s => string.Equals(s.Name, previousSelection, StringComparison.Ordinal));
        if (_selectedSkinIndex < 0 && _skins.Count > 0)
        {
            _selectedSkinIndex = 0;
        }

        _status = _skins.Count == 0 ? "No valid skins found." : $"Loaded {_skins.Count} skin(s).";
    }

    private void LoadSavedSelection()
    {
        string selectionFilePath = Path.Combine(SkinsRootPath, SelectionFileName);
        if (!File.Exists(selectionFilePath))
        {
            return;
        }

        string selectionName = File.ReadAllText(selectionFilePath).Trim();
        int selectionIndex = _skins.FindIndex(s => string.Equals(s.Name, selectionName, StringComparison.Ordinal));
        if (selectionIndex < 0)
        {
            _status = $"Saved skin '{selectionName}' not found.";
            return;
        }

        _selectedSkinIndex = selectionIndex;
        ApplySelectedSkin();
    }

    private void ApplySelectedSkin()
    {
        if (_selectedSkinIndex < 0 || _selectedSkinIndex >= _skins.Count)
        {
            _status = "Select a skin first.";
            return;
        }

        SkinOption selectedSkin = _skins[_selectedSkinIndex];
        File.Copy(selectedSkin.SpriteSheetPath, ActiveSpriteSheetPath, overwrite: true);
        File.WriteAllText(Path.Combine(SkinsRootPath, SelectionFileName), selectedSkin.Name);
        _status = $"Applied skin: {selectedSkin.Name}";
    }

    private static string? FindSpriteSheet(string skinDirectory)
    {
        foreach (string candidate in Directory.EnumerateFiles(skinDirectory))
        {
            string fileName = Path.GetFileName(candidate);
            if (SpriteSheetNames.Any(expected => string.Equals(expected, fileName, StringComparison.OrdinalIgnoreCase)))
            {
                return candidate;
            }
        }

        return null;
    }

    private sealed class SkinOption
    {
        public SkinOption(string name, string spriteSheetPath)
        {
            Name = name;
            SpriteSheetPath = spriteSheetPath;
        }

        public string Name { get; }
        public string SpriteSheetPath { get; }
    }
}
