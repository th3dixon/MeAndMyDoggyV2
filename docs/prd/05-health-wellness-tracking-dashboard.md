# Story 5: Health & Wellness Tracking Dashboard

## Overview
This section defines the requirements for implementing comprehensive health and wellness tracking features that integrate with existing medical records, AI health recommendations, and appointment systems to provide intelligent health insights, medication reminders, and wellness recommendations.

## Story Definition
**As a pet owner,**  
**I want to track and monitor my dog's health and wellness through intelligent dashboard widgets,**  
**so that I can proactively manage their care, receive timely reminders, and make informed decisions about their health.**

## Technical Context

### Existing Health Infrastructure
- **Medical Records**: DogMedicalRecord entities with comprehensive health tracking
- **AI Integration**: Google Gemini AI for health recommendations and insights
- **Appointment System**: PetServiceBooking with veterinary service integration
- **Service Providers**: Veterinary and health service provider network
- **Data Models**: Comprehensive health data including vaccinations, medications, treatments

### Health Tracking Requirements
- Integration with existing medical record database schema
- AI-powered health insights using established Gemini integration
- Medication and vaccination reminder system
- Health trend analysis and visualization
- Emergency health alert system

## Functional Requirements

### FR5.1: Health Dashboard Integration
- **Requirement**: Comprehensive health overview widget displaying current health status and key metrics
- **Integration Point**: Existing DogMedicalRecord entities and health data models
- **Implementation**: Health summary widget with trend visualization and alert indicators
- **Success Criteria**: Health dashboard displays accurate data from existing medical records with real-time updates

### FR5.2: AI-Powered Health Insights
- **Requirement**: Intelligent health recommendations and insights using existing Gemini AI integration
- **Integration Point**: Current Google Gemini API implementation and health data analysis
- **Implementation**: AI health analysis service leveraging existing patterns and data models
- **Success Criteria**: AI insights provided within 5 seconds with >85% user-perceived relevance

### FR5.3: Medication and Vaccination Reminders
- **Requirement**: Automated reminder system for medications, vaccinations, and health check-ups
- **Integration Point**: Existing notification system and calendar integration
- **Implementation**: Smart reminder engine with configurable schedules and escalation
- **Success Criteria**: 95% reminder delivery accuracy with zero missed critical health events

### FR5.4: Health Trend Analysis and Visualization
- **Requirement**: Visual health trend tracking with charts and progress indicators
- **Integration Point**: Existing health data and analytics infrastructure
- **Implementation**: Interactive health charts with trend analysis and milestone tracking
- **Success Criteria**: Health trends update in real-time with intuitive visualization

## Health Tracking Specifications

### Health Dashboard Widget Components
```typescript
interface HealthDashboardConfig {
  dogId: string;
  displayMetrics: ('weight' | 'vaccinations' | 'medications' | 'symptoms' | 'vitals')[];
  timeRange: 'week' | 'month' | 'quarter' | 'year';
  showTrends: boolean;
  showAlerts: boolean;
  showRecommendations: boolean;
  size: 'small' | 'medium' | 'large';
}

interface HealthMetric {
  type: string;
  value: number | string;
  unit?: string;
  recordedAt: Date;
  normal: boolean;
  trend: 'improving' | 'stable' | 'concerning';
  notes?: string;
}

interface HealthAlert {
  id: string;
  type: 'medication_due' | 'vaccination_overdue' | 'symptom_concern' | 'checkup_reminder';
  severity: 'low' | 'medium' | 'high' | 'critical';
  title: string;
  description: string;
  actionRequired: string;
  dueDate?: Date;
  dogId: string;
}
```

### AI Health Analysis Integration
```typescript
interface AIHealthInsight {
  id: string;
  dogId: string;
  insightType: 'recommendation' | 'warning' | 'trend_analysis' | 'preventive_care';
  title: string;
  description: string;
  confidence: number;
  dataPoints: string[];
  recommendedActions: string[];
  generatedAt: Date;
  expiresAt?: Date;
}

interface HealthAnalysisRequest {
  dogId: string;
  timeRange: number; // days
  includeSymptoms: boolean;
  includeMedications: boolean;
  includeVaccinations: boolean;
  includeVitalSigns: boolean;
}
```

## Technical Implementation

### Health Service Integration
```csharp
// Health Dashboard Service
public interface IHealthDashboardService
{
    Task<HealthSummary> GetHealthSummaryAsync(string dogId, TimeRange timeRange);
    Task<List<HealthAlert>> GetHealthAlertsAsync(string userId);
    Task<List<AIHealthInsight>> GetAIHealthInsightsAsync(string dogId);
    Task<HealthTrendData> GetHealthTrendsAsync(string dogId, string metricType, TimeRange timeRange);
    Task<bool> CreateHealthReminderAsync(string userId, HealthReminder reminder);
    Task<bool> LogHealthEventAsync(string userId, string dogId, HealthEvent healthEvent);
}

public class HealthDashboardService : IHealthDashboardService
{
    private readonly IDogMedicalRecordRepository _medicalRecordRepository;
    private readonly IAIHealthAnalysisService _aiHealthService;
    private readonly IReminderService _reminderService;
    private readonly INotificationService _notificationService;
    
    public async Task<HealthSummary> GetHealthSummaryAsync(string dogId, TimeRange timeRange)
    {
        var medicalRecords = await _medicalRecordRepository
            .GetRecordsByDogIdAndDateRangeAsync(dogId, timeRange.StartDate, timeRange.EndDate);
        
        var healthSummary = new HealthSummary
        {
            DogId = dogId,
            OverallStatus = CalculateOverallHealthStatus(medicalRecords),
            RecentVaccinations = GetRecentVaccinations(medicalRecords),
            CurrentMedications = GetCurrentMedications(medicalRecords),
            UpcomingReminders = await GetUpcomingReminders(dogId),
            HealthAlerts = await GetHealthAlerts(dogId),
            LastCheckup = GetLastCheckupDate(medicalRecords)
        };
        
        return healthSummary;
    }
    
    public async Task<List<AIHealthInsight>> GetAIHealthInsightsAsync(string dogId)
    {
        var healthData = await GatherHealthDataForAI(dogId);
        var insights = await _aiHealthService.AnalyzeHealthDataAsync(healthData);
        
        // Store insights for tracking and follow-up
        await StoreAIInsights(dogId, insights);
        
        return insights;
    }
    
    private async Task<HealthAnalysisData> GatherHealthDataForAI(string dogId)
    {
        var dog = await _dogRepository.GetByIdAsync(dogId);
        var medicalRecords = await _medicalRecordRepository.GetByDogIdAsync(dogId);
        var behaviorRecords = await _behaviorRepository.GetByDogIdAsync(dogId);
        
        return new HealthAnalysisData
        {
            DogProfile = dog,
            MedicalHistory = medicalRecords,
            BehaviorHistory = behaviorRecords,
            CurrentMedications = GetCurrentMedications(medicalRecords),
            VaccinationHistory = GetVaccinationHistory(medicalRecords)
        };
    }
}

// AI Health Analysis Service
public interface IAIHealthAnalysisService
{
    Task<List<AIHealthInsight>> AnalyzeHealthDataAsync(HealthAnalysisData data);
    Task<HealthRiskAssessment> AssessHealthRisksAsync(string dogId);
    Task<List<PreventiveCareRecommendation>> GetPreventiveCareRecommendationsAsync(string dogId);
}

public class AIHealthAnalysisService : IAIHealthAnalysisService
{
    private readonly IGeminiAIService _geminiService;
    private readonly IHealthKnowledgeBase _knowledgeBase;
    
    public async Task<List<AIHealthInsight>> AnalyzeHealthDataAsync(HealthAnalysisData data)
    {
        var prompt = BuildHealthAnalysisPrompt(data);
        var aiResponse = await _geminiService.GenerateHealthInsightsAsync(prompt);
        
        var insights = ParseAIResponse(aiResponse);
        
        // Validate insights against knowledge base
        var validatedInsights = await ValidateInsights(insights, data);
        
        return validatedInsights;
    }
    
    private string BuildHealthAnalysisPrompt(HealthAnalysisData data)
    {
        var prompt = $@"
            Please analyze the health data for {data.DogProfile.Name}, a {data.DogProfile.Age}-year-old {data.DogProfile.Breed}.
            
            Recent Medical History:
            {string.Join("\n", data.MedicalHistory.Select(r => $"- {r.RecordDate}: {r.Description}"))}
            
            Current Medications:
            {string.Join("\n", data.CurrentMedications.Select(m => $"- {m.Name}: {m.Dosage}"))}
            
            Vaccination Status:
            {string.Join("\n", data.VaccinationHistory.Select(v => $"- {v.VaccineName}: {v.DateAdministered}"))}
            
            Please provide:
            1. Overall health assessment
            2. Any concerning trends or patterns
            3. Preventive care recommendations
            4. Medication adherence observations
            5. Vaccination schedule recommendations
            
            Focus on actionable insights that will help the owner maintain their dog's health.
        ";
        
        return prompt;
    }
}
```

### Health Dashboard Frontend Components
```vue
<!-- Health Overview Widget -->
<template>
  <WidgetBase
    :title="'Health Overview'"
    :loading="loading"
    :error="error"
    class="health-overview-widget"
  >
    <div class="health-status-summary">
      <!-- Overall Health Status -->
      <div class="health-status-indicator">
        <div class="status-circle" :class="overallStatusClass">
          <HealthIcon :status="healthSummary.overallStatus" />
        </div>
        <div class="status-text">
          <h3>{{ healthSummary.overallStatus.title }}</h3>
          <p>{{ healthSummary.overallStatus.description }}</p>
        </div>
      </div>
      
      <!-- Health Alerts -->
      <div v-if="healthAlerts.length > 0" class="health-alerts">
        <h4>Active Alerts</h4>
        <HealthAlert
          v-for="alert in healthAlerts"
          :key="alert.id"
          :alert="alert"
          @action-taken="handleAlertAction"
        />
      </div>
      
      <!-- Key Metrics -->
      <div class="health-metrics">
        <h4>Key Metrics</h4>
        <div class="metrics-grid">
          <HealthMetric
            v-for="metric in keyMetrics"
            :key="metric.type"
            :metric="metric"
            :show-trend="true"
          />
        </div>
      </div>
      
      <!-- AI Insights -->
      <div v-if="aiInsights.length > 0" class="ai-insights">
        <h4>AI Health Insights</h4>
        <AIInsightCard
          v-for="insight in aiInsights"
          :key="insight.id"
          :insight="insight"
          @view-details="viewInsightDetails"
        />
      </div>
    </div>
    
    <!-- Quick Actions -->
    <div class="health-quick-actions">
      <button @click="logHealthEvent" class="quick-action-btn">
        <PlusIcon />
        Log Health Event
      </button>
      <button @click="viewFullHistory" class="quick-action-btn">
        <HistoryIcon />
        View Full History
      </button>
      <button @click="scheduleCheckup" class="quick-action-btn">
        <CalendarIcon />
        Schedule Checkup
      </button>
    </div>
  </WidgetBase>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useHealthDashboard } from '@/composables/useHealthDashboard';
import { useAIHealthInsights } from '@/composables/useAIHealthInsights';

interface Props {
  dogId: string;
  configuration: HealthDashboardConfig;
}

const props = defineProps<Props>();

const {
  healthSummary,
  healthAlerts,
  keyMetrics,
  loading,
  error,
  refreshHealthData
} = useHealthDashboard(props.dogId);

const {
  aiInsights,
  loadInsights,
  refreshInsights
} = useAIHealthInsights(props.dogId);

const overallStatusClass = computed(() => ({
  'status-excellent': healthSummary.value?.overallStatus.level === 'excellent',
  'status-good': healthSummary.value?.overallStatus.level === 'good',
  'status-concerning': healthSummary.value?.overallStatus.level === 'concerning',
  'status-critical': healthSummary.value?.overallStatus.level === 'critical'
}));

const logHealthEvent = () => {
  // Open health event logging modal
};

const viewFullHistory = () => {
  // Navigate to full health history view
};

const scheduleCheckup = () => {
  // Open appointment scheduling for veterinary checkup
};

const handleAlertAction = (alertId: string, actionType: string) => {
  // Handle alert action (dismiss, schedule, etc.)
};

const viewInsightDetails = (insightId: string) => {
  // Open detailed view of AI insight
};

onMounted(async () => {
  await Promise.all([
    refreshHealthData(),
    loadInsights()
  ]);
});
</script>

<style scoped>
.health-status-indicator {
  @apply flex items-center gap-4 mb-6;
}

.status-circle {
  @apply w-12 h-12 rounded-full flex items-center justify-center;
}

.status-excellent {
  @apply bg-green-100 text-green-600;
}

.status-good {
  @apply bg-blue-100 text-blue-600;
}

.status-concerning {
  @apply bg-yellow-100 text-yellow-600;
}

.status-critical {
  @apply bg-red-100 text-red-600;
}

.metrics-grid {
  @apply grid grid-cols-2 gap-3;
}

.health-quick-actions {
  @apply flex gap-2 mt-4 pt-4 border-t border-gray-200;
}

.quick-action-btn {
  @apply flex items-center gap-2 px-3 py-2 bg-yellow-50 text-yellow-700;
  @apply border border-yellow-200 rounded-md hover:bg-yellow-100;
  @apply transition-colors duration-150;
}
</style>
```

### Health Reminders System
```vue
<!-- Health Reminders Widget -->
<template>
  <WidgetBase
    :title="'Health Reminders'"
    :loading="loading"
    :error="error"
    class="health-reminders-widget"
  >
    <div class="reminders-container">
      <!-- Overdue Reminders -->
      <div v-if="overdueReminders.length > 0" class="overdue-section">
        <h4 class="section-title overdue">Overdue</h4>
        <ReminderCard
          v-for="reminder in overdueReminders"
          :key="reminder.id"
          :reminder="reminder"
          :is-overdue="true"
          @complete="completeReminder"
          @reschedule="rescheduleReminder"
        />
      </div>
      
      <!-- Upcoming Reminders -->
      <div class="upcoming-section">
        <h4 class="section-title">Upcoming</h4>
        <ReminderCard
          v-for="reminder in upcomingReminders"
          :key="reminder.id"
          :reminder="reminder"
          @complete="completeReminder"
          @reschedule="rescheduleReminder"
        />
      </div>
      
      <!-- Add New Reminder -->
      <div class="add-reminder-section">
        <button @click="showAddReminder = true" class="add-reminder-btn">
          <PlusIcon />
          Add Health Reminder
        </button>
      </div>
    </div>
    
    <!-- Add Reminder Modal -->
    <AddReminderModal
      v-if="showAddReminder"
      :dog-id="dogId"
      @created="onReminderCreated"
      @close="showAddReminder = false"
    />
  </WidgetBase>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useHealthReminders } from '@/composables/useHealthReminders';

interface Props {
  dogId: string;
}

const props = defineProps<Props>();

const {
  reminders,
  loading,
  error,
  loadReminders,
  completeReminder: markComplete,
  rescheduleReminder: updateSchedule,
  createReminder
} = useHealthReminders(props.dogId);

const showAddReminder = ref(false);

const overdueReminders = computed(() =>
  reminders.value.filter(r => r.isOverdue)
);

const upcomingReminders = computed(() =>
  reminders.value.filter(r => !r.isOverdue && !r.isCompleted)
);

const completeReminder = async (reminderId: string) => {
  await markComplete(reminderId);
  await loadReminders();
};

const rescheduleReminder = async (reminderId: string, newDate: Date) => {
  await updateSchedule(reminderId, newDate);
  await loadReminders();
};

const onReminderCreated = async () => {
  showAddReminder.value = false;
  await loadReminders();
};

onMounted(async () => {
  await loadReminders();
});
</script>
```

## Acceptance Criteria

### AC5.1: Health Overview Display
- **Given**: A user with a dog that has existing medical records
- **When**: They view the health dashboard widget
- **Then**: Current health status, recent metrics, and key indicators are displayed accurately
- **And**: Health data is sourced from existing DogMedicalRecord entities without performance impact

### AC5.2: AI Health Insights
- **Given**: Sufficient health data exists for a dog
- **When**: AI health analysis is triggered
- **Then**: Relevant health insights and recommendations are generated within 5 seconds
- **And**: AI insights are actionable and contextually relevant to the dog's current health status

### AC5.3: Health Reminders
- **Given**: A user has set up medication and vaccination schedules
- **When**: A reminder becomes due
- **Then**: The user receives timely notifications through existing notification channels
- **And**: Reminder status is accurately tracked and updated based on user actions

### AC5.4: Health Trend Visualization
- **Given**: Historical health data for a dog
- **When**: The user views health trends
- **Then**: Visual charts show meaningful health patterns and trends over time
- **And**: Users can interact with charts to view specific data points and time ranges

### AC5.5: Emergency Health Alerts
- **Given**: Health data indicating a potential concern
- **When**: The system detects patterns suggesting immediate attention is needed
- **Then**: High-priority alerts are generated and delivered immediately
- **And**: Users receive clear guidance on recommended actions

## Integration Verification Points

### IV5.1: Medical Records Integration
- Verify health dashboard accurately displays data from existing DogMedicalRecord entities
- Confirm health data queries maintain existing database performance baselines
- Ensure health event logging integrates seamlessly with current medical record storage

### IV5.2: AI Service Integration
- Verify AI health insights use established Google Gemini API integration patterns
- Confirm AI analysis respects existing rate limiting and cost management controls
- Ensure AI recommendations align with existing health knowledge base and veterinary guidelines

### IV5.3: Notification System Integration
- Verify health reminders integrate with existing notification delivery mechanisms
- Confirm reminder notifications respect user notification preferences and channels
- Ensure health alerts follow established priority and escalation patterns

## Non-Functional Requirements

### NFR5.1: Performance
- Health dashboard must load within 2 seconds with full health summary
- AI health insights must generate within 5 seconds of request
- Health trend calculations must complete within 1 second for standard time ranges

### NFR5.2: Accuracy and Reliability
- Health data must maintain 100% accuracy with source medical records
- AI health insights must achieve >85% user-perceived relevance rating
- Health reminder delivery must achieve 99% success rate for critical reminders

### NFR5.3: Data Privacy and Security
- All health data access must follow existing medical data privacy requirements
- AI health analysis must not store or transmit sensitive data beyond established patterns
- Health insights must be stored securely with appropriate data retention policies

## Testing Strategy

### Unit Tests
- Health data aggregation and summary calculation logic
- AI health insight generation and validation
- Health reminder scheduling and notification logic
- Health trend analysis and visualization calculations

### Integration Tests
- End-to-end health dashboard data flow from medical records to display
- AI health analysis integration with existing Gemini service
- Health reminder integration with notification delivery system
- Health event logging and medical record storage

### User Acceptance Tests
- Health dashboard usability and information clarity
- AI insight relevance and actionability assessment
- Health reminder effectiveness and user satisfaction
- Emergency health alert responsiveness and accuracy

## Dependencies

### Existing Systems
- DogMedicalRecord entities and medical data storage
- Google Gemini AI integration and health analysis capabilities
- Notification system and delivery mechanisms
- User authentication and data access authorization

### Required Integrations
- Health dashboard widget framework and real-time updates
- AI health analysis service with existing Gemini patterns
- Health reminder engine with notification system integration
- Health trend visualization with interactive charting components

## Success Metrics

### Functional Metrics
- 100% accuracy of health data display compared to source medical records
- >85% user satisfaction rating for AI health insight relevance
- 99% success rate for critical health reminder delivery

### Performance Metrics
- Health dashboard load time <2 seconds for comprehensive health summary
- AI health insight generation time <5 seconds
- Health trend visualization rendering time <1 second

### User Experience Metrics
- Health dashboard usage rate >70% of users with dogs having medical records
- AI health insight action rate >40% (users taking recommended actions)
- Health reminder completion rate >80% for scheduled medications and appointments