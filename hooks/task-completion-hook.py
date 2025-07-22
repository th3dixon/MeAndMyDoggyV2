#!/usr/bin/env python3
"""
Task Completion Hook
Automatically runs code validation when all high-priority tasks are completed.
Integrates with Claude Code's TodoWrite tool to maintain code quality standards.
"""

import os
import sys
import json
import subprocess
from pathlib import Path
from typing import Dict, List, Any

def check_task_completion_status(todos: List[Dict[str, Any]]) -> Dict[str, Any]:
    """Check task completion status for triggering validation"""
    # Separate tasks by priority
    high_priority_tasks = [task for task in todos if task.get('priority') == 'high']
    medium_priority_tasks = [task for task in todos if task.get('priority') == 'medium']
    low_priority_tasks = [task for task in todos if task.get('priority') == 'low']
    
    # Count completed tasks
    completed_high_tasks = [task for task in high_priority_tasks if task.get('status') == 'completed']
    completed_medium_tasks = [task for task in medium_priority_tasks if task.get('status') == 'completed']
    completed_low_tasks = [task for task in low_priority_tasks if task.get('status') == 'completed']
    
    all_tasks = len(todos)
    completed_tasks = len([task for task in todos if task.get('status') == 'completed'])
    
    # Check if all tasks of any priority level are completed
    all_high_completed = len(completed_high_tasks) == len(high_priority_tasks) and len(high_priority_tasks) > 0
    all_medium_completed = len(completed_medium_tasks) == len(medium_priority_tasks) and len(medium_priority_tasks) > 0
    all_low_completed = len(completed_low_tasks) == len(low_priority_tasks) and len(low_priority_tasks) > 0
    all_tasks_completed = completed_tasks == all_tasks and all_tasks > 0
    
    # Trigger validation if any complete priority level or all tasks done
    should_trigger_validation = all_high_completed or all_medium_completed or all_low_completed or all_tasks_completed
    
    return {
        'should_trigger_validation': should_trigger_validation,
        'all_tasks_completed': all_tasks_completed,
        'all_high_priority_completed': all_high_completed,
        'all_medium_priority_completed': all_medium_completed,
        'all_low_priority_completed': all_low_completed,
        'overall_completion_rate': (completed_tasks / all_tasks * 100) if all_tasks > 0 else 0,
        'total_tasks': all_tasks,
        'completed_tasks': completed_tasks,
        'high_priority_tasks': len(high_priority_tasks),
        'completed_high_priority': len(completed_high_tasks),
        'medium_priority_tasks': len(medium_priority_tasks),
        'completed_medium_priority': len(completed_medium_tasks),
        'low_priority_tasks': len(low_priority_tasks),
        'completed_low_priority': len(completed_low_tasks)
    }

def run_code_validation() -> Dict[str, Any]:
    """Run the code validation hook"""
    print("\n🔧 Running automatic code validation...")
    print("=" * 50)
    
    project_root = Path.cwd()
    hook_path = project_root / "hooks" / "code-validation-hook.py"
    
    if not hook_path.exists():
        return {
            'success': False,
            'message': f'Code validation hook not found at: {hook_path}',
            'compliance_score': None
        }
    
    try:
        # Run the code validation hook
        result = subprocess.run([
            sys.executable, str(hook_path)
        ], capture_output=True, text=True, cwd=project_root)
        
        # Check if validation report was generated
        report_path = project_root / "CODE_VALIDATION_REPORT.md"
        
        if report_path.exists():
            print(f"📊 Code validation report generated: {report_path}")
            
            # Try to extract compliance score from the output
            compliance_score = None
            if "Compliance Score:" in result.stdout:
                try:
                    for line in result.stdout.split('\n'):
                        if "Compliance Score:" in line:
                            score_text = line.split("Compliance Score:")[1].strip()
                            compliance_score = float(score_text.split('/')[0])
                            break
                except:
                    pass
            
            return {
                'success': result.returncode == 0,
                'message': 'Code validation completed',
                'compliance_score': compliance_score,
                'has_errors': result.returncode != 0,
                'output': result.stdout,
                'errors': result.stderr if result.stderr else None
            }
        else:
            return {
                'success': False,
                'message': 'Code validation hook ran but no report was generated',
                'compliance_score': None,
                'output': result.stdout,
                'errors': result.stderr
            }
            
    except Exception as e:
        return {
            'success': False,
            'message': f'Error running code validation: {str(e)}',
            'compliance_score': None
        }

def generate_task_completion_summary(task_status: Dict[str, Any], validation_result: Dict[str, Any]) -> str:
    """Generate a completion summary with validation results"""
    lines = []
    lines.append("# Task Completion Summary")
    lines.append("")
    
    # Task completion status
    lines.append("## Task Status")
    lines.append(f"- **Total Tasks**: {task_status['total_tasks']}")
    lines.append(f"- **Completed Tasks**: {task_status['completed_tasks']}")
    lines.append(f"- **Overall Progress**: {task_status['overall_completion_rate']:.1f}%")
    lines.append("")
    lines.append("### By Priority Level:")
    lines.append(f"- **High Priority**: {task_status['completed_high_priority']}/{task_status['high_priority_tasks']} completed")
    lines.append(f"- **Medium Priority**: {task_status['completed_medium_priority']}/{task_status['medium_priority_tasks']} completed")
    lines.append(f"- **Low Priority**: {task_status['completed_low_priority']}/{task_status['low_priority_tasks']} completed")
    lines.append("")
    
    # Completion triggers
    lines.append("### Completion Status:")
    if task_status['all_tasks_completed']:
        lines.append("- ALL TASKS COMPLETED!")
    elif task_status['all_high_priority_completed']:
        lines.append("- All high-priority tasks completed")
    elif task_status['all_medium_priority_completed']:
        lines.append("- All medium-priority tasks completed")
    elif task_status['all_low_priority_completed']:
        lines.append("- All low-priority tasks completed")
    else:
        lines.append("- Tasks still in progress")
    lines.append("")
    
    # Code validation status
    lines.append("## Code Quality Validation")
    if validation_result['success']:
        compliance_score = validation_result.get('compliance_score', 'Unknown')
        lines.append(f"- **Status**: PASSED")
        lines.append(f"- **Compliance Score**: {compliance_score}/100")
        lines.append(f"- **Quality Gate**: {'PASSED' if not validation_result.get('has_errors') else 'FAILED'}")
    else:
        lines.append(f"- **Status**: FAILED")
        lines.append(f"- **Issue**: {validation_result.get('message', 'Unknown error')}")
        
    lines.append("")
    
    # Recommendations
    lines.append("## Next Steps")
    if task_status['should_trigger_validation']:
        if validation_result['success'] and not validation_result.get('has_errors'):
            lines.append("SUCCESS: Task milestone reached with passing code validation!")
            lines.append("- Code quality standards are maintained")
            lines.append("- Ready for next phase or deployment")
        else:
            lines.append("WARNING: Task milestone reached but code validation found issues")
            lines.append("- Review the CODE_VALIDATION_REPORT.md for detailed findings")
            lines.append("- Address critical errors before proceeding")
            lines.append("- Consider refactoring any code quality warnings")
    else:
        lines.append("INFO: Tasks in progress - validation will run automatically when a priority level is completed")
    
    return "\n".join(lines)

def main():
    """Main execution when called directly"""
    print("🚀 Task Completion Hook")
    
    # This would be called by Claude Code integration
    # For now, just run validation on demand
    validation_result = run_code_validation()
    
    print("\n📊 Validation Results:")
    if validation_result['success']:
        print("✅ Code validation passed!")
        if validation_result.get('compliance_score'):
            print(f"📈 Compliance Score: {validation_result['compliance_score']}/100")
    else:
        print("❌ Code validation failed!")
        print(f"❗ Error: {validation_result.get('message')}")
    
    return validation_result['success']

if __name__ == "__main__":
    success = main()
    sys.exit(0 if success else 1)