#!/usr/bin/env python3
import os
from pathlib import Path

def main():
    print("Testing validation hook...")
    project_root = Path(os.getcwd())
    print(f"Project root: {project_root}")
    
    # Find C# files
    cs_files = list(project_root.rglob('*.cs'))
    print(f"Found {len(cs_files)} C# files")
    
    for file in cs_files[:5]:
        print(f"  - {file.relative_to(project_root)}")
        
    # Test the multiple class detection on AuthResponseDto
    auth_response_file = project_root / "src/API/MeAndMyDog.API/Models/DTOs/Auth/AuthResponseDto.cs"
    if auth_response_file.exists():
        print(f"\nTesting {auth_response_file.relative_to(project_root)}...")
        with open(auth_response_file, 'r', encoding='utf-8') as f:
            content = f.read()
            
        import re
        class_pattern = r'^\s*(?:\/\/\/.*\n)*\s*(?:\[.*\]\s*)*public\s+(?:partial\s+)?class\s+(\w+)'
        matches = list(re.finditer(class_pattern, content, re.MULTILINE))
        
        print(f"Found {len(matches)} public classes:")
        for match in matches:
            line_number = content[:match.start()].count('\n') + 1
            print(f"  - {match.group(1)} at line {line_number}")
            
        if len(matches) > 1:
            print("❌ VIOLATION: Multiple classes in one file!")
        else:
            print("✅ OK: Single class per file")

if __name__ == "__main__":
    main()