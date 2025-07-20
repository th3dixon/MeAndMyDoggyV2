<!------------------------------------------------------------------------------------
   Add Rules to this file or a short description and have Kiro refine them for you:   
-------------------------------------------------------------------------------------> 

# Documentation Standards

## XML Documentation Requirements

### Mandatory Documentation
All public classes, interfaces, methods, and properties MUST have XML documentation comments.

### Required Elements
1. **Classes and Interfaces**:
   ```csharp
   /// <summary>
   /// Brief description of what the class does
   /// </summary>
   public class ExampleClass
   ```

2. **Methods**:
   ```csharp
   /// <summary>
   /// Brief description of what the method does
   /// </summary>
   /// <param name="parameterName">Description of the parameter</param>
   /// <returns>Description of what is returned</returns>
   public async Task<string> ExampleMethod(string parameterName)
   ```

3. **Properties**:
   ```csharp
   /// <summary>
   /// Description of what the property represents
   /// </summary>
   public string ExampleProperty { get; set; }
   ```

### Documentation Standards
- Use clear, concise language
- Explain the purpose, not the implementation
- Include parameter descriptions for all parameters
- Document return values for non-void methods
- Document exceptions that may be thrown using `<exception>` tags
- Use proper grammar and punctuation

### Project Configuration
Enable XML documentation generation in the project file:
```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <DocumentationFile>bin\$(Configuration)\$(TargetFramework)\$(AssemblyName).xml</DocumentationFile>
</PropertyGroup>
```

### Enforcement
- All code reviews must verify XML documentation is present
- Build warnings should be treated as errors for missing documentation
- Use code analysis rules to enforce documentation standards


# Architecture Validation Engine Specification

## Overview

The Architecture Validation Engine is a comprehensive code quality and architecture compliance system that analyzes codebases across multiple programming languages to ensure adherence to architectural patterns, coding standards, and best practices.

## Core Capabilities

### Multi-Language Support ✅
- **C#**: Entity Framework, ASP.NET Core, dependency injection patterns
- **Python**: SQLAlchemy, Django, FastAPI, Flask patterns
- **TypeScript/JavaScript**: Node.js, React, Angular, Express patterns
- **Java**: Spring Framework, JPA, Maven/Gradle patterns
- **Go**: Standard library patterns, dependency management

### Validation Categories ✅
1. **Architecture Compliance**: Directory structure, separation of concerns, dependency direction
2. **Interface Location**: Interface organization, contract-first development, implementation separation
3. **Implementation Completeness**: Placeholder detection, production readiness, code quality
4. **Database Design**: Entity naming conventions, relationship validation, API design patterns
5. **Code Quality**: Complexity metrics, maintainability, security patterns
6. **Class Naming**: Duplicate class detection, ambiguous naming, single-responsibility enforcement
7. **Enum Standards**: Database-compatible enum values, naming conventions, suffix validation

## Architecture Patterns Supported

### API Architecture ✅
```
src/
├── controllers/     # HTTP request handlers
├── services/        # Business logic layer
├── models/          # Data models and entities
├── interfaces/      # Contract definitions
├── middleware/      # Request processing pipeline
├── utils/           # Shared utilities
└── tests/           # Test suites
```

**Validation Rules:**
- Controllers must not contain business logic
- Services must implement interfaces
- Models must follow entity naming conventions
- Middleware must be stateless and composable

### Single-Tenant Architecture ✅
```
src/
├── core/           # Core business logic
├── infrastructure/ # External dependencies
├── application/    # Use cases and workflows
├── domain/         # Domain entities and rules
├── presentation/   # UI and API layers
└── shared/         # Cross-cutting concerns
```

**Validation Rules:**
- Domain layer must not depend on infrastructure
- Application layer orchestrates domain operations
- Infrastructure implements domain interfaces
- Presentation layer handles user interaction only
- No repositories must be used as Entity Framework is a repository on its own. Services should be preferred over repositories.

### Multi-Tenant Architecture ✅
```
src/
├── tenant/         # Tenant-specific logic
├── shared/         # Shared across tenants
├── isolation/      # Tenant isolation mechanisms
├── provisioning/   # Tenant lifecycle management
├── billing/        # Tenant billing and usage
└── security/       # Multi-tenant security
```

**Validation Rules:**
- Tenant data must be isolated
- Shared resources must be tenant-aware
- Security policies must be tenant-scoped
- Billing must track per-tenant usage

### Microservices Architecture ✅
```
services/
├── user-service/
├── auth-service/
├── billing-service/
├── notification-service/
├── gateway/        # API Gateway
├── shared/         # Shared libraries
└── infrastructure/ # Deployment configs
```

**Validation Rules:**
- Services must be independently deployable
- No shared databases between services
- Communication through well-defined APIs
- Each service owns its data

## Validation Engine Components

### 1. File Pattern Analyzer ✅

**Language Detection:**
```python
def detect_language(file_path: str) -> str:
    """Detect programming language from file extension and content"""
    extension_map = {
        '.cs': 'csharp',
        '.py': 'python',
        '.ts': 'typescript',
        '.js': 'javascript',
        '.java': 'java',
        '.go': 'go'
    }
```

**Pattern Matching:**
```python
def extract_patterns(content: str, language: str) -> Dict[str, List]:
    """Extract architectural patterns from source code"""
    patterns = {
        'classes': extract_class_definitions(content, language),
        'interfaces': extract_interface_definitions(content, language),
        'methods': extract_method_definitions(content, language),
        'dependencies': extract_dependencies(content, language)
    }
```

### 2. Interface Location Validator ✅

**Interface Detection:**
```python
def validate_interface_location(self, file_path: str, content: str) -> List[ArchitectureViolation]:
    """Validate interface organization and location"""
    interfaces = self._extract_interfaces(content)
    violations = []
    
    for interface in interfaces:
        # Check directory structure
        if not self._is_in_interface_directory(file_path):
            violations.append(self._create_misplaced_interface_violation(file_path, interface))
        
        # Check naming conventions
        if not self._follows_interface_naming(interface['name'], file_path):
            violations.append(self._create_naming_violation(file_path, interface))
```

**Interface-First Development Detection:**
```python
def _detect_interface_first_development(self, interface_info: Dict) -> float:
    """Detect if this is interface-first development using multiple indicators"""
    indicators = [
        self._has_comprehensive_documentation(interface_info),
        self._has_pending_todo_comments(interface_info),
        self._referenced_in_tests(interface_info),
        self._recently_created(interface_info),
        self._has_multiple_method_signatures(interface_info),
        self._used_in_type_definitions(interface_info),
        self._has_detailed_method_docs(interface_info)
    ]
    return sum(indicators) / len(indicators)
```

### 3. Implementation Completeness Validator ✅

**Placeholder Detection Patterns:**
```python
placeholder_patterns = [
    # Exception patterns
    r'throw new NotImplementedException',
    r'raise NotImplementedError',
    r'panic\("not implemented"\)',
    
    # Comment patterns
    r'\/\/\s*TODO',
    r'#\s*TODO',
    r'\/\*\s*TODO.*?\*\/',
    
    # Placeholder implementations
    r'return null;?\s*\/\/.*placeholder',
    r'pass\s*#.*placeholder',
    r'{}.*\/\/.*placeholder',
    
    # AI-generated patterns
    r'\/\/.*In production you.*',
    r'#.*Replace with actual implementation',
    r'\/\/.*This is a placeholder',
    
    # Mock/stub patterns
    r'return.*"Coming soon"',
    r'return.*MockData',
    r'\.stub\(\)',
    
    # Debug patterns
    r'Console\.WriteLine\(',
    r'print\("DEBUG',
    r'console\.log\(',
    r'fmt\.Println\(',
    
    # Hardcoded test data
    r'"test@example\.com"',
    r'"password123"',
    r'"localhost"',
    r'"admin".*"admin"'
]
```

**Production Readiness Check:**
```python
def validate_production_readiness(self, content: str, file_path: str) -> List[ArchitectureViolation]:
    """Comprehensive production readiness validation"""
    violations = []
    
    # 1. Check for incomplete implementations
    violations.extend(self._check_placeholder_patterns(content, file_path))
    
    # 2. Validate error handling
    violations.extend(self._check_error_handling(content, file_path))
    
    # 3. Check for hardcoded values
    violations.extend(self._check_hardcoded_values(content, file_path))
    
    # 4. Validate logging and monitoring
    violations.extend(self._check_observability(content, file_path))
    
    return violations
```

### 4. Database Entity Validator ✅

**Entity Detection Patterns:**
```python
def _extract_csharp_entities(self, content: str) -> List[Dict[str, Any]]:
    """Extract C# entity class information"""
    # Class pattern with attributes
    class_pattern = r'(?:\[Table\([^\]]+\)\]\s*)?public\s+class\s+(\w+)(?:\s*:\s*([^{]+))?\s*\{'
    
    # Property patterns
    property_patterns = [
        r'\[Key\]\s*public\s+(\w+(?:<[^>]+>)?(?:\?)?)\s+(\w+)\s*\{\s*get;\s*set;\s*\}',
        r'public\s+(\w+(?:<[^>]+>)?(?:\?)?)\s+(\w+)\s*\{\s*get;\s*set;\s*\}'
    ]
```

**Primary Key Naming Validation:**
```python
def _validate_primary_key_naming(self, file_path: str, class_name: str, properties: List[Dict]) -> List[ArchitectureViolation]:
    """Validate primary key naming conventions"""
    # Distinguish between DTOs and entities
    is_dto = self._is_dto_class(class_name)
    
    if is_dto:
        # DTOs should replicate entity property names (TenantDto -> TenantId)
        expected_pk_name = self._get_dto_primary_key_name(class_name)
    else:
        # Entities follow EntityNameId pattern (User -> UserId)
        expected_pk_name = f"{class_name}Id"
    
    primary_keys = [prop for prop in properties if prop.get('is_primary_key', False)]
    
    for pk_prop in primary_keys:
        if pk_prop['name'] != expected_pk_name:
            violations.append(self._create_pk_naming_violation(
                file_path, pk_prop, expected_pk_name, is_dto
            ))
```

**API-Facing Entity Detection:**
```python
def _is_api_facing_entity(self, class_name: str, inheritance: str, content: str) -> bool:
    """Determine if entity is used in public-facing APIs"""
    api_indicators = [
        'controller', 'api', 'dto', 'request', 'response', 
        'public', 'external', 'client', 'web'
    ]
    
    # Check usage patterns
    api_usage_patterns = [
        rf'\b{class_name}\b.*(?:Controller|Api|Endpoint)',
        rf'public.*\b{class_name}\b',
        rf'@.*Api.*\b{class_name}\b'
    ]
    
    return (
        any(indicator in class_name.lower() for indicator in api_indicators) or
        any(re.search(pattern, content, re.IGNORECASE) for pattern in api_usage_patterns)
    )
```

### 5. Class Naming Conflict Validator ✅

**Duplicate Class Detection:**
```python
def _validate_class_naming_conflicts(self, project_path: Path, structure: Dict) -> Tuple[List[ArchitectureViolation], List[str]]:
    """Validate class naming conflicts, duplicates, and organizational patterns"""
    violations = []
    rules_applied = []
    
    # Multi-language class extraction
    all_classes = {}  # file_path -> [class_info]
    
    for file_type, file_paths in structure.get('files_by_type', {}).items():
        for file_path in file_paths:
            if self._should_skip_file(file_path):
                continue
                
            full_path = project_path / file_path
            try:
                with open(full_path, 'r', encoding='utf-8', errors='ignore') as f:
                    content = f.read()
                    
                # Extract classes based on file type
                classes = self._extract_classes_by_language(content, file_type)
                if classes:
                    all_classes[file_path] = classes
                    
            except Exception as e:
                continue
    
    # 1. Check for duplicate classes across files
    violations.extend(self._check_duplicate_classes(all_classes))
    rules_applied.append("class_duplicate_detection")
    
    # 2. Check for ambiguous class names
    violations.extend(self._check_ambiguous_class_names(all_classes))
    rules_applied.append("class_ambiguous_naming")
    
    # 3. Check for multiple classes per file
    violations.extend(self._check_multiple_classes_per_file(all_classes))
    rules_applied.append("class_single_per_file")
    
    return violations, rules_applied
```

**Multi-Language Class Extraction:**
```python
def _extract_classes_by_language(self, content: str, file_type: str) -> List[Dict[str, Any]]:
    """Extract class definitions from different programming languages"""
    extractors = {
        '.cs': self._extract_csharp_classes,
        '.py': self._extract_python_classes,
        '.ts': self._extract_typescript_classes,
        '.js': self._extract_typescript_classes,
        '.java': self._extract_java_classes,
        '.go': self._extract_go_classes
    }
    
    extractor = extractors.get(file_type)
    if extractor:
        return extractor(content)
    return []

def _extract_csharp_classes(self, content: str) -> List[Dict[str, Any]]:
    """Extract C# class definitions including partial classes"""
    classes = []
    # Regular classes and partial classes
    class_pattern = r'(?:public\s+)?(?:partial\s+)?class\s+(\w+)(?:\s*:\s*([^{]+))?\s*\{'
    
    for match in re.finditer(class_pattern, content, re.MULTILINE):
        classes.append({
            'name': match.group(1),
            'inheritance': match.group(2).strip() if match.group(2) else '',
            'line_number': content[:match.start()].count('\n') + 1,
            'is_partial': 'partial' in match.group(0),
            'access_modifier': 'public' if 'public' in match.group(0) else 'internal'
        })
    
    return classes
```

**Ambiguous Name Detection:**
```python
def _check_ambiguous_class_names(self, all_classes: Dict[str, List[Dict]]) -> List[ArchitectureViolation]:
    """Check for confusingly similar class names using Levenshtein distance"""
    violations = []
    all_class_names = []
    
    # Collect all class names with their locations
    for file_path, classes in all_classes.items():
        for class_info in classes:
            all_class_names.append({
                'name': class_info['name'],
                'file_path': file_path,
                'line_number': class_info.get('line_number')
            })
    
    # Check for ambiguous names
    ambiguous_patterns = ['Manager', 'Service', 'Helper', 'Utility', 'Handler']
    
    for class_info in all_class_names:
        class_name = class_info['name']
        
        # Check against ambiguous patterns
        if any(pattern in class_name for pattern in ambiguous_patterns):
            violations.append(ArchitectureViolation(
                file_path=class_info['file_path'],
                line_number=class_info['line_number'],
                violation_type='ambiguous_class_name',
                severity='warning',
                rule_id='class_naming_clarity',
                message=f'Class name "{class_name}" is ambiguous and may cause confusion',
                suggestion=f'Use a more descriptive and specific name for class "{class_name}"'
            ))
        
        # Check for similar names using Levenshtein distance
        for other_class in all_class_names:
            if (class_info != other_class and 
                self._calculate_name_similarity(class_name, other_class['name']) > 0.8):
                violations.append(ArchitectureViolation(
                    file_path=class_info['file_path'],
                    line_number=class_info['line_number'],
                    violation_type='ambiguous_class_name',
                    severity='warning',
                    rule_id='class_naming_similarity',
                    message=f'Class "{class_name}" is very similar to "{other_class["name"]}"',
                    suggestion=f'Consider renaming to avoid confusion with similar class names'
                ))
    
    return violations
```

### 6. Enum Definition Validator ✅

**Database Compatibility Validation:**
```python
def _validate_enum_definitions(self, project_path: Path, structure: Dict) -> Tuple[List[ArchitectureViolation], List[str]]:
    """Validate enum definitions for database compatibility and naming conventions"""
    violations = []
    rules_applied = []
    
    # Extract enums from all supported file types
    all_enums = {}  # file_path -> [enum_info]
    
    for file_type, file_paths in structure.get('files_by_type', {}).items():
        for file_path in file_paths:
            if self._should_skip_file(file_path):
                continue
                
            full_path = project_path / file_path
            try:
                with open(full_path, 'r', encoding='utf-8', errors='ignore') as f:
                    content = f.read()
                    
                # Extract enums based on file type
                enums = self._extract_enums_by_language(content, file_type)
                if enums:
                    all_enums[file_path] = enums
                    
            except Exception as e:
                continue
    
    # 1. Validate enum values start from 1 (database compatibility)
    violations.extend(self._check_enum_values_start_from_one(all_enums))
    rules_applied.append("enum_database_compatibility")
    
    # 2. Validate enum naming suffixes
    violations.extend(self._check_enum_naming_suffixes(all_enums))
    rules_applied.append("enum_naming_convention")
    
    return violations, rules_applied

def _check_enum_values_start_from_one(self, all_enums: Dict[str, List[Dict]]) -> List[ArchitectureViolation]:
    """Validate that enum values start from 1 for database compatibility"""
    violations = []
    
    for file_path, enums in all_enums.items():
        for enum_info in enums:
            if enum_info['first_value'] == 0:
                violations.append(ArchitectureViolation(
                    file_path=file_path,
                    line_number=enum_info['line_number'],
                    violation_type='enum_values_start_from_one',
                    severity='error',
                    rule_id='enum_database_compatibility',
                    message=f'Enum "{enum_info["name"]}" first value is 0, should start from 1',
                    suggestion='Change the first enum value to 1 to ensure database lookup table compatibility. This prevents issues with 0-based indexing in database foreign key relationships.'
                ))
    
    return violations

def _check_enum_naming_suffixes(self, all_enums: Dict[str, List[Dict]]) -> List[ArchitectureViolation]:
    """Validate enum naming conventions with recognizable suffixes"""
    violations = []
    
    valid_suffixes = ['Type', 'Status', 'Mode', 'Kind', 'Category', 'State', 'Flag', 'Option']
    
    for file_path, enums in all_enums.items():
        for enum_info in enums:
            if not self._has_valid_enum_suffix(enum_info['name'], valid_suffixes):
                violations.append(ArchitectureViolation(
                    file_path=file_path,
                    line_number=enum_info['line_number'],
                    violation_type='enum_naming_suffix',
                    severity='warning',
                    rule_id='enum_naming_convention',
                    message=f'Enum "{enum_info["name"]}" should have a recognizable suffix ({", ".join(valid_suffixes)})',
                    suggestion=f'Rename enum to include a suffix like "{enum_info["name"]}Type" or "{enum_info["name"]}Status" to distinguish it from regular classes'
                ))
    
    return violations
```

## Configuration System

### Default Configuration ✅
```json
{
  "project": {
    "architectureType": "api",
    "projectName": "Project Name",
    "projectSlug": "project-name"
  },
  "validation": {
    "quality_gates": {
      "enforce_one_class_per_file": true,
      "require_core_interfaces": true,
      "mandate_method_overloads": true,
      "architecture_compliance": true
    },
    "interface_validation": {
      "detect_interface_first_development": true,
      "allow_unused_interfaces_in_development": true,
      "interface_first_likelihood_threshold": 0.3,
      "enforce_interface_separation": true,
      "require_interface_documentation": false
    },
    "implementation_completeness": {
      "enforce_full_implementation": true,
      "check_placeholder_patterns": true,
      "production_ready_validation": true,
      "check_test_files": true,
      "check_hardcoded_data": true,
      "check_debug_code": true
    },
    "database_entities": {
      "enforce_entity_validation": true,
      "require_entity_prefix_primary_key": true,
      "prohibit_simple_id_property": true,
      "require_public_id_for_api_entities": true,
      "prohibit_guid_id_naming": true,
      "allow_inherited_public_id": true
    },
    "class_naming": {
      "detect_duplicate_classes": true,
      "detect_ambiguous_names": true,
      "enforce_single_class_per_file": true,
      "ambiguous_name_patterns": ["Manager", "Service", "Helper", "Utility", "Handler"],
      "similarity_threshold": 0.8
    },
    "enum_validation": {
      "enforce_start_from_one": true,
      "require_naming_suffix": true,
      "valid_suffixes": ["Type", "Status", "Mode", "Kind", "Category", "State", "Flag", "Option"],
      "database_compatibility": true
    }
  }
}
```

### Custom Rule Configuration ✅
```json
{
  "custom_rules": {
    "naming_conventions": {
      "interface_prefix": "I",
      "entity_suffix": "",
      "service_suffix": "Service",
      "controller_suffix": "Controller"
    },
    "architecture_patterns": {
      "allowed_dependencies": {
        "controllers": ["services", "models"],
        "services": ["repositories", "models"],
        "repositories": ["models"]
      },
      "prohibited_dependencies": {
        "models": ["controllers", "services"],
        "repositories": ["controllers"]
      }
    },
    "quality_thresholds": {
      "cyclomatic_complexity": 10,
      "method_length": 50,
      "class_length": 500,
      "parameter_count": 5
    }
  }
}
```

## Validation Rules Engine

### Rule Definition Format ✅
```python
class ValidationRule:
    def __init__(self, rule_id: str, severity: str, category: str):
        self.rule_id = rule_id
        self.severity = severity  # 'error', 'warning', 'info'
        self.category = category  # 'architecture', 'quality', 'security'
        
    def validate(self, file_path: str, content: str, config: Dict) -> List[ArchitectureViolation]:
        """Abstract method for rule validation"""
        raise NotImplementedError
```

### Built-in Rule Categories ✅

#### Architecture Rules
- **api_directory_structure**: Validates API project directory organization
- **interface_location**: Ensures interfaces are properly organized
- **dependency_direction**: Validates dependency flow between layers
- **separation_of_concerns**: Checks for mixed responsibilities in classes

#### Quality Rules
- **implementation_completeness**: Detects incomplete implementations
- **cyclomatic_complexity**: Measures code complexity
- **method_length**: Validates method size limits
- **naming_conventions**: Enforces consistent naming patterns

#### Database Rules
- **entity_primary_key_naming**: Validates primary key naming conventions
- **entity_simple_id_prohibited**: Prohibits simple "Id" property names
- **entity_public_id_required**: Requires PublicId for API entities
- **entity_guid_id_prohibited**: Prohibits "GuidId" naming pattern

#### Class Naming Rules
- **class_duplicate_detection**: Detects duplicate class definitions across files
- **class_ambiguous_naming**: Identifies confusingly similar class names
- **class_single_per_file**: Enforces one class per file for maintainability
- **class_naming_conflicts**: Comprehensive class naming validation

#### Enum Standards Rules
- **enum_database_compatibility**: Ensures enum values start from 1 for database compatibility
- **enum_naming_convention**: Validates enum naming suffixes (Type, Status, Mode, etc.)
- **enum_first_value_validation**: Enforces first enum value = 1 for lookup table consistency

#### Security Rules
- **secrets_detection**: Identifies hardcoded secrets and credentials
- **sql_injection_patterns**: Detects potential SQL injection vulnerabilities
- **xss_prevention**: Validates XSS prevention measures
- **authentication_patterns**: Ensures proper authentication implementation

## Violation Reporting

### Violation Object Structure ✅
```python
@dataclass
class ArchitectureViolation:
    file_path: str
    line_number: Optional[int]
    violation_type: str
    severity: str  # 'error', 'warning', 'info'
    rule_id: str
    message: str
    suggestion: str
    metadata: Optional[Dict[str, Any]] = None
```

### Report Formats ✅

#### JSON Report
```json
{
  "architecture_type": "api",
  "compliance_score": 85.7,
  "violations_count": 23,
  "violations_by_type": {
    "architecture": 5,
    "quality": 12,
    "database": 6
  },
  "violations": [
    {
      "file_path": "src/controllers/UserController.cs",
      "line_number": 45,
      "violation_type": "implementation_placeholder",
      "severity": "error",
      "rule_id": "implementation_completeness",
      "message": "Incomplete implementation detected: throw new NotImplementedException()",
      "suggestion": "Replace with actual implementation"
    }
  ]
}
```

#### Markdown Report
```markdown
# Architecture Validation Report

**Architecture Type**: API  
**Compliance Score**: 85.7/100  
**Violations Found**: 23

## Summary
- **Critical Issues**: 3
- **Warnings**: 15
- **Info**: 5

## Violations

### 🔴 Critical: Incomplete Implementation
**File**: `src/controllers/UserController.cs:45`  
**Rule**: implementation_completeness  
**Message**: Incomplete implementation detected: throw new NotImplementedException()  
**Suggestion**: Replace with actual implementation
```

## Performance Characteristics

### Processing Speed ✅
- **Small Projects** (< 100 files): < 5 seconds
- **Medium Projects** (100-1000 files): < 30 seconds  
- **Large Projects** (1000+ files): < 2 minutes
- **Enterprise Projects** (10,000+ files): < 10 minutes

### Memory Usage ✅
- **Base Memory**: 50MB for validator engine
- **Per File**: ~1KB additional memory usage
- **Large File Handling**: Streaming analysis for files > 1MB
- **Caching**: Intelligent caching of parsed ASTs and patterns

### Scalability Considerations ✅
- **Parallel Processing**: Multi-threaded analysis for independent files
- **Incremental Analysis**: Only analyze changed files in CI/CD
- **Distributed Processing**: Support for distributed validation across multiple nodes
- **Result Caching**: Cache validation results with file content hashing

## Integration Points

### CI/CD Integration ✅
```yaml
# GitHub Actions Example
- name: Architecture Validation
  run: |
    python architecture-validator.py \
      --project-path . \
      --architecture api \
      --format json \
      --output validation-report.json
    
    # Fail build if critical issues found
    if [ $(jq '.violations[] | select(.severity=="error") | length' validation-report.json) -gt 0 ]; then
      exit 1
    fi
```

### IDE Integration ✅
```json
{
  "vscode.extension": "claude-architecture-validator",
  "settings": {
    "validator.enableLiveValidation": true,
    "validator.configPath": ".claude/config/validation-config.json",
    "validator.excludePatterns": ["**/node_modules/**", "**/bin/**"]
  }
}
```

### Pre-commit Hook Integration ✅
```bash
#!/bin/bash
# .git/hooks/pre-commit
echo "Running architecture validation..."

python .claude/automation/architecture-validator.py \
  --project-path . \
  --architecture api \
  --format markdown \
  --output /tmp/validation-report.md

# Check for critical violations
if grep -q "🔴 Critical" /tmp/validation-report.md; then
  echo "❌ Architecture validation failed - critical issues found"
  cat /tmp/validation-report.md
  exit 1
fi

echo "✅ Architecture validation passed"
```

## Extension and Customization

### Custom Rule Development ✅
```python
class CustomSecurityRule(ValidationRule):
    def __init__(self):
        super().__init__(
            rule_id="custom_security_check",
            severity="error",
            category="security"
        )
    
    def validate(self, file_path: str, content: str, config: Dict) -> List[ArchitectureViolation]:
        violations = []
        
        # Custom validation logic
        if self._contains_hardcoded_secrets(content):
            violations.append(ArchitectureViolation(
                file_path=file_path,
                violation_type="security_violation",
                severity="error",
                rule_id=self.rule_id,
                message="Hardcoded secrets detected",
                suggestion="Use environment variables or secret management"
            ))
        
        return violations
```

### Plugin Architecture ✅
```python
class ValidationPlugin:
    def register_rules(self) -> List[ValidationRule]:
        """Register custom rules with the validation engine"""
        return [
            CustomSecurityRule(),
            CustomNamingRule(),
            CustomArchitectureRule()
        ]
    
    def post_process_violations(self, violations: List[ArchitectureViolation]) -> List[ArchitectureViolation]:
        """Post-process violations for custom reporting"""
        return violations
```

This comprehensive validation engine specification ensures consistent, high-quality code architecture across all supported languages and frameworks while providing extensive customization capabilities for specific project requirements.