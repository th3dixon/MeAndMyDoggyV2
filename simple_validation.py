#!/usr/bin/env python3
"""Simple validation test"""

import os
import re
from pathlib import Path

def check_multiple_classes(file_path):
    """Check for multiple classes in a file"""
    try:
        with open(file_path, 'r', encoding='utf-8', errors='ignore') as f:
            content = f.read()
            
        class_pattern = r'^\s*(?:\/\/\/.*\n)*\s*(?:\[.*\]\s*)*public\s+(?:partial\s+)?class\s+(\w+)'
        matches = list(re.finditer(class_pattern, content, re.MULTILINE))
        
        if len(matches) > 1:
            class_names = [match.group(1) for match in matches]
            return {
                'violation': True,
                'classes': class_names,
                'file': str(file_path)
            }
    except Exception as e:
        print(f"Error checking {file_path}: {e}")
    
    return {'violation': False}

def main():
    print("🔍 Simple Validation Check")
    print("=" * 40)
    
    # Check specific files we know have issues
    problem_files = [
        "src/API/MeAndMyDog.API/Models/DTOs/Auth/AuthResponseDto.cs",
        "src/API/MeAndMyDog.API/Models/DTOs/ServiceCatalog/ServiceCategoryDto.cs"
    ]
    
    violations_found = 0
    
    for file_path in problem_files:
        full_path = Path(file_path)
        if full_path.exists():
            print(f"\n📁 Checking: {file_path}")
            result = check_multiple_classes(full_path)
            
            if result['violation']:
                violations_found += 1
                print(f"❌ VIOLATION: Multiple classes found: {', '.join(result['classes'])}")
                for class_name in result['classes']:
                    print(f"   💡 Suggestion: Move {class_name} to its own file: {class_name}.cs")
            else:
                print("✅ OK: Single class per file")
        else:
            print(f"⚠️  File not found: {file_path}")
    
    print(f"\n📊 Summary:")
    print(f"   Files checked: {len(problem_files)}")
    print(f"   Violations found: {violations_found}")
    
    if violations_found > 0:
        print("\n❌ Code standards violations detected!")
        print("Please fix the multiple class violations before proceeding.")
        return 1
    else:
        print("\n✅ All files comply with single class per file standard!")
        return 0

if __name__ == "__main__":
    exit(main())