#!/usr/bin/env python3
"""
MeAndMyDog Code Validation Hook
Validates code against established standards and architecture rules.
Checks files modified within the last 2 days for comprehensive coverage.
"""

import os
import re
import json
import subprocess
from datetime import datetime, timedelta
from pathlib import Path
from typing import List, Dict, Any, Tuple
from dataclasses import dataclass

@dataclass
class ValidationViolation:
    file_path: str
    line_number: int
    violation_type: str
    severity: str  # 'error', 'warning', 'info'
    rule_id: str
    message: str
    suggestion: str

class CodeValidator:
    def __init__(self, project_root: str):
        self.project_root = Path(project_root)
        self.violations: List[ValidationViolation] = []
        
    def get_recently_modified_files(self, days: int = 2) -> List[str]:
        """Get files modified within the last N days using git"""
        try:
            # Get files modified in the last N days
            since_date = (datetime.now() - timedelta(days=days)).strftime('%Y-%m-%d')
            result = subprocess.run([
                'git', 'log', '--since', since_date, '--name-only', '--pretty=format:', '--diff-filter=AM'
            ], capture_output=True, text=True, cwd=self.project_root)
            
            if result.returncode == 0:
                files = [f.strip() for f in result.stdout.split('\n') if f.strip()]
                # Filter for C# and TypeScript files
                return [f for f in files if f.endswith(('.cs', '.ts', '.tsx')) and os.path.exists(self.project_root / f)]
            else:
                print(f"Git command failed: {result.stderr}")
                return []
        except Exception as e:
            print(f"Error getting modified files: {e}")
            return []

    def validate_single_class_per_file(self, file_path: str) -> List[ValidationViolation]:
        """Validate that each file contains only one public class"""
        violations = []
        
        try:
            with open(self.project_root / file_path, 'r', encoding='utf-8', errors='ignore') as f:
                content = f.read()
                
            # Find all public class declarations
            class_pattern = r'^\s*(?:\/\/\/.*\n)*\s*(?:\[.*\]\s*)*public\s+(?:partial\s+)?class\s+(\w+)'
            matches = list(re.finditer(class_pattern, content, re.MULTILINE))
            
            if len(matches) > 1:
                class_names = [match.group(1) for match in matches]
                for i, match in enumerate(matches):
                    line_number = content[:match.start()].count('\n') + 1
                    violations.append(ValidationViolation(
                        file_path=file_path,
                        line_number=line_number,
                        violation_type='multiple_classes_per_file',
                        severity='error',
                        rule_id='class_single_per_file',
                        message=f'Multiple public classes found in file: {", ".join(class_names)}',
                        suggestion=f'Move class "{match.group(1)}" to its own file: {match.group(1)}.cs'
                    ))
                    
        except Exception as e:
            print(f"Error validating {file_path}: {e}")
            
        return violations    def 
validate_xml_documentation(self, file_path: str) -> List[ValidationViolation]:
        """Validate XML documentation for public members"""
        violations = []
        
        if not file_path.endswith('.cs'):
            return violations
            
        try:
            with open(self.project_root / file_path, 'r', encoding='utf-8', errors='ignore') as f:
                content = f.read()
                
            # Find public classes without XML documentation
            class_pattern = r'(?:^|\n)(\s*)(?:(?!\/\/\/).*\n)*\s*public\s+(?:partial\s+)?class\s+(\w+)'
            for match in re.finditer(class_pattern, content, re.MULTILINE):
                preceding_text = content[:match.start()]
                lines_before = preceding_text.split('\n')[-5:]  # Check last 5 lines
                
                has_xml_doc = any('///' in line for line in lines_before)
                if not has_xml_doc:
                    line_number = preceding_text.count('\n') + 1
                    violations.append(ValidationViolation(
                        file_path=file_path,
                        line_number=line_number,
                        violation_type='missing_xml_documentation',
                        severity='warning',
                        rule_id='xml_documentation_required',
                        message=f'Public class "{match.group(2)}" missing XML documentation',
                        suggestion=f'Add /// <summary> documentation above class "{match.group(2)}"'
                    ))
                    
        except Exception as e:
            print(f"Error validating XML documentation in {file_path}: {e}")
            
        return violations

    def validate_console_statements(self, file_path: str) -> List[ValidationViolation]:
        """Validate no console statements in production code"""
        violations = []
        
        if not file_path.endswith(('.ts', '.tsx')):
            return violations
            
        try:
            with open(self.project_root / file_path, 'r', encoding='utf-8', errors='ignore') as f:
                content = f.read()
                
            # Find console statements
            console_pattern = r'console\.(log|error|warn|info|debug)\s*\('
            for match in re.finditer(console_pattern, content, re.MULTILINE):
                line_number = content[:match.start()].count('\n') + 1
                violations.append(ValidationViolation(
                    file_path=file_path,
                    line_number=line_number,
                    violation_type='production_debug_code',
                    severity='error',
                    rule_id='no_console_statements',
                    message=f'Console statement found: console.{match.group(1)}()',
                    suggestion='Replace with proper logging service or remove for production'
                ))
                
        except Exception as e:
            print(f"Error validating console statements in {file_path}: {e}")
            
        return violations

    def validate_hardcoded_secrets(self, file_path: str) -> List[ValidationViolation]:
        """Validate no hardcoded secrets or test data"""
        violations = []
        
        try:
            with open(self.project_root / file_path, 'r', encoding='utf-8', errors='ignore') as f:
                content = f.read()
                
            # Patterns for hardcoded secrets
            secret_patterns = [
                (r'password123', 'hardcoded test password'),
                (r'admin.*admin', 'hardcoded admin credentials'),
                (r'test@example\.com', 'hardcoded test email'),
                (r'default-secret-key', 'hardcoded default secret'),
                (r'localhost.*password', 'hardcoded localhost password')
            ]
            
            for pattern, description in secret_patterns:
                for match in re.finditer(pattern, content, re.IGNORECASE):
                    line_number = content[:match.start()].count('\n') + 1
                    violations.append(ValidationViolation(
                        file_path=file_path,
                        line_number=line_number,
                        violation_type='hardcoded_secret',
                        severity='error',
                        rule_id='no_hardcoded_secrets',
                        message=f'Hardcoded secret detected: {description}',
                        suggestion='Move to configuration or environment variables'
                    ))
                    
        except Exception as e:
            print(f"Error validating secrets in {file_path}: {e}")
            
        return violations    def
 validate_incomplete_implementations(self, file_path: str) -> List[ValidationViolation]:
        """Validate no incomplete implementations"""
        violations = []
        
        if not file_path.endswith('.cs'):
            return violations
            
        try:
            with open(self.project_root / file_path, 'r', encoding='utf-8', errors='ignore') as f:
                content = f.read()
                
            # Patterns for incomplete implementations
            incomplete_patterns = [
                (r'throw new NotImplementedException', 'NotImplementedException found'),
                (r'\/\/\s*TODO', 'TODO comment found'),
                (r'\/\/\s*FIXME', 'FIXME comment found'),
                (r'\/\/\s*HACK', 'HACK comment found')
            ]
            
            for pattern, description in incomplete_patterns:
                for match in re.finditer(pattern, content, re.IGNORECASE):
                    line_number = content[:match.start()].count('\n') + 1
                    violations.append(ValidationViolation(
                        file_path=file_path,
                        line_number=line_number,
                        violation_type='incomplete_implementation',
                        severity='error',
                        rule_id='no_incomplete_implementations',
                        message=f'Incomplete implementation: {description}',
                        suggestion='Complete the implementation before committing'
                    ))
                    
        except Exception as e:
            print(f"Error validating implementations in {file_path}: {e}")
            
        return violations

    def validate_all_files(self, files: List[str]) -> Dict[str, Any]:
        """Run all validations on the provided files"""
        all_violations = []
        
        for file_path in files:
            print(f"Validating: {file_path}")
            
            # Run all validation rules
            file_violations = []
            file_violations.extend(self.validate_single_class_per_file(file_path))
            file_violations.extend(self.validate_xml_documentation(file_path))
            file_violations.extend(self.validate_console_statements(file_path))
            file_violations.extend(self.validate_hardcoded_secrets(file_path))
            file_violations.extend(self.validate_incomplete_implementations(file_path))
            
            all_violations.extend(file_violations)
            
        # Categorize violations
        violations_by_severity = {
            'error': [v for v in all_violations if v.severity == 'error'],
            'warning': [v for v in all_violations if v.severity == 'warning'],
            'info': [v for v in all_violations if v.severity == 'info']
        }
        
        # Calculate compliance score
        total_files = len(files)
        files_with_errors = len(set(v.file_path for v in violations_by_severity['error']))
        compliance_score = max(0, 100 - (files_with_errors / max(total_files, 1)) * 100)
        
        return {
            'total_files_checked': total_files,
            'total_violations': len(all_violations),
            'violations_by_severity': {
                'errors': len(violations_by_severity['error']),
                'warnings': len(violations_by_severity['warning']),
                'info': len(violations_by_severity['info'])
            },
            'compliance_score': round(compliance_score, 1),
            'violations': all_violations
        }    def g
enerate_report(self, results: Dict[str, Any]) -> str:
        """Generate a formatted validation report"""
        violations = results['violations']
        
        report = []
        report.append("# Code Validation Report")
        report.append(f"**Files Checked**: {results['total_files_checked']}")
        report.append(f"**Compliance Score**: {results['compliance_score']}/100")
        report.append(f"**Total Violations**: {results['total_violations']}")
        report.append("")
        
        # Summary by severity
        report.append("## Summary")
        report.append(f"- **Errors**: {results['violations_by_severity']['errors']}")
        report.append(f"- **Warnings**: {results['violations_by_severity']['warnings']}")
        report.append(f"- **Info**: {results['violations_by_severity']['info']}")
        report.append("")
        
        if violations:
            report.append("## Violations")
            
            # Group by severity
            for severity in ['error', 'warning', 'info']:
                severity_violations = [v for v in violations if v.severity == severity]
                if severity_violations:
                    icon = {'error': '🔴', 'warning': '⚠️', 'info': 'ℹ️'}[severity]
                    report.append(f"### {icon} {severity.title()} Issues")
                    
                    for violation in severity_violations:
                        report.append(f"**File**: `{violation.file_path}:{violation.line_number}`")
                        report.append(f"**Rule**: {violation.rule_id}")
                        report.append(f"**Message**: {violation.message}")
                        report.append(f"**Suggestion**: {violation.suggestion}")
                        report.append("")
        else:
            report.append("## ✅ No Violations Found")
            report.append("All checked files comply with coding standards!")
            
        return "\n".join(report)

def main():
    """Main execution function"""
    print("Starting validation hook...")
    project_root = os.getcwd()
    print(f"Working directory: {project_root}")
    validator = CodeValidator(project_root)
    
    print("🔍 MeAndMyDog Code Validation Hook")
    print("=" * 50)
    
    # Get recently modified files
    print("📁 Getting recently modified files (last 2 days)...")
    modified_files = validator.get_recently_modified_files(days=2)
    
    if not modified_files:
        print("ℹ️  No recently modified files found via git. Checking all C# and TypeScript files...")
        # Fallback to checking all files if git doesn't work
        all_files = []
        for ext in ['.cs', '.ts', '.tsx']:
            for file_path in Path(project_root).rglob(f'*{ext}'):
                if ('node_modules' not in str(file_path) and 
                    'bin' not in str(file_path) and 
                    'obj' not in str(file_path) and
                    '.git' not in str(file_path)):
                    all_files.append(str(file_path.relative_to(project_root)))
        print(f"Found {len(all_files)} total files")
        modified_files = all_files[:20]  # Limit to first 20 files for testing
        
    if not modified_files:
        print("ℹ️  No C# or TypeScript files found.")
        return
        
    print(f"📋 Found {len(modified_files)} files to validate:")
    for file in modified_files[:10]:  # Show first 10
        print(f"   - {file}")
    if len(modified_files) > 10:
        print(f"   ... and {len(modified_files) - 10} more")
    print()
    
    # Run validation
    print("🔍 Running validation checks...")
    results = validator.validate_all_files(modified_files)
    
    # Generate and save report
    report = validator.generate_report(results)
    
    # Save report to file
    report_file = Path(project_root) / "CODE_VALIDATION_REPORT.md"
    with open(report_file, 'w', encoding='utf-8') as f:
        f.write(report)
    
    print(f"📊 Validation complete! Report saved to: {report_file}")
    print()
    print("📈 Results Summary:")
    print(f"   Compliance Score: {results['compliance_score']}/100")
    print(f"   Total Violations: {results['total_violations']}")
    print(f"   Errors: {results['violations_by_severity']['errors']}")
    print(f"   Warnings: {results['violations_by_severity']['warnings']}")
    
    # Exit with error code if critical issues found
    if results['violations_by_severity']['errors'] > 0:
        print("\n❌ Critical issues found! Please fix errors before proceeding.")
        exit(1)
    else:
        print("\n✅ No critical issues found!")
        exit(0)

if __name__ == "__main__":
    main()