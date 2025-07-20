/**
 * Modal Manager for MeAndMyDoggy
 * Provides a consistent modal dialog system across the application
 */

(function(window) {
    'use strict';

    // Modal HTML template
    const modalTemplate = `
        <div id="app-modal" class="fixed inset-0 z-50 hidden overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
            <!-- Backdrop -->
            <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity modal-backdrop"></div>
            
            <!-- Modal Container -->
            <div class="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0">
                <div class="modal-content relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg">
                    <!-- Modal Header -->
                    <div class="modal-header bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4">
                        <div class="sm:flex sm:items-start">
                            <div class="modal-icon-container mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full sm:mx-0 sm:h-10 sm:w-10">
                                <i class="modal-icon"></i>
                            </div>
                            <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left flex-1">
                                <h3 class="modal-title text-base font-semibold leading-6 text-gray-900" id="modal-title"></h3>
                                <div class="mt-2">
                                    <p class="modal-message text-sm text-gray-500"></p>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <!-- Modal Actions -->
                    <div class="modal-actions bg-gray-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6">
                        <!-- Actions will be inserted here -->
                    </div>
                </div>
            </div>
        </div>
    `;

    // Modal type configurations
    const modalTypes = {
        success: {
            iconClass: 'fas fa-check text-white',
            iconBgClass: 'bg-green-100',
            iconColorClass: 'text-green-600',
            primaryButtonClass: 'bg-green-600 hover:bg-green-700 focus:ring-green-500'
        },
        error: {
            iconClass: 'fas fa-exclamation text-white',
            iconBgClass: 'bg-red-100',
            iconColorClass: 'text-red-600',
            primaryButtonClass: 'bg-red-600 hover:bg-red-700 focus:ring-red-500'
        },
        warning: {
            iconClass: 'fas fa-exclamation-triangle text-white',
            iconBgClass: 'bg-yellow-100',
            iconColorClass: 'text-yellow-600',
            primaryButtonClass: 'bg-yellow-600 hover:bg-yellow-700 focus:ring-yellow-500'
        },
        info: {
            iconClass: 'fas fa-info text-white',
            iconBgClass: 'bg-blue-100',
            iconColorClass: 'text-blue-600',
            primaryButtonClass: 'bg-blue-600 hover:bg-blue-700 focus:ring-blue-500'
        },
        confirm: {
            iconClass: 'fas fa-question text-white',
            iconBgClass: 'bg-pet-orange/10',
            iconColorClass: 'text-pet-orange',
            primaryButtonClass: 'bg-pet-orange hover:bg-orange-600 focus:ring-pet-orange'
        }
    };

    // Modal manager class
    class ModalManager {
        constructor() {
            this.modal = null;
            this.focusableElements = [];
            this.lastFocusedElement = null;
            this.boundKeyHandler = this.handleKeyDown.bind(this);
            this.init();
        }

        init() {
            // Inject modal template into body if not exists
            if (!document.getElementById('app-modal')) {
                document.body.insertAdjacentHTML('beforeend', modalTemplate);
            }
            this.modal = document.getElementById('app-modal');
            
            // Add click handler to backdrop
            const backdrop = this.modal.querySelector('.modal-backdrop');
            backdrop.addEventListener('click', () => this.hide());
        }

        show(options) {
            const defaults = {
                title: 'Notification',
                message: '',
                type: 'info',
                actions: [
                    { text: 'OK', primary: true, action: () => this.hide() }
                ],
                closeable: true
            };

            const config = Object.assign({}, defaults, options);
            
            // Store last focused element
            this.lastFocusedElement = document.activeElement;

            // Update modal content
            this.updateModalContent(config);

            // Show modal with animation
            this.modal.classList.remove('hidden');
            this.modal.offsetHeight; // Force reflow
            
            // Add animation classes
            const backdrop = this.modal.querySelector('.modal-backdrop');
            const content = this.modal.querySelector('.modal-content');
            
            backdrop.classList.add('opacity-0');
            content.classList.add('opacity-0', 'translate-y-4', 'sm:translate-y-0', 'sm:scale-95');
            
            setTimeout(() => {
                backdrop.classList.remove('opacity-0');
                content.classList.remove('opacity-0', 'translate-y-4', 'sm:translate-y-0', 'sm:scale-95');
            }, 10);

            // Set up keyboard handling
            document.addEventListener('keydown', this.boundKeyHandler);

            // Trap focus
            this.trapFocus();

            return new Promise((resolve) => {
                this.resolvePromise = resolve;
            });
        }

        hide(result = null) {
            const backdrop = this.modal.querySelector('.modal-backdrop');
            const content = this.modal.querySelector('.modal-content');
            
            // Add animation classes
            backdrop.classList.add('opacity-0');
            content.classList.add('opacity-0', 'translate-y-4', 'sm:translate-y-0', 'sm:scale-95');
            
            setTimeout(() => {
                this.modal.classList.add('hidden');
                
                // Clean up
                document.removeEventListener('keydown', this.boundKeyHandler);
                
                // Restore focus
                if (this.lastFocusedElement) {
                    this.lastFocusedElement.focus();
                }
                
                // Resolve promise if exists
                if (this.resolvePromise) {
                    this.resolvePromise(result);
                    this.resolvePromise = null;
                }
            }, 200);
        }

        updateModalContent(config) {
            const typeConfig = modalTypes[config.type] || modalTypes.info;
            
            // Update icon
            const iconContainer = this.modal.querySelector('.modal-icon-container');
            const icon = this.modal.querySelector('.modal-icon');
            iconContainer.className = `modal-icon-container mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full sm:mx-0 sm:h-10 sm:w-10 ${typeConfig.iconBgClass}`;
            icon.className = `modal-icon ${typeConfig.iconClass}`;

            // Update content
            this.modal.querySelector('.modal-title').textContent = config.title;
            this.modal.querySelector('.modal-message').textContent = config.message;

            // Update actions
            const actionsContainer = this.modal.querySelector('.modal-actions');
            actionsContainer.innerHTML = '';

            config.actions.forEach((action, index) => {
                const button = document.createElement('button');
                button.type = 'button';
                button.textContent = action.text;
                
                if (action.primary) {
                    button.className = `inline-flex w-full justify-center rounded-md px-3 py-2 text-sm font-semibold text-white shadow-sm sm:ml-3 sm:w-auto ${typeConfig.primaryButtonClass}`;
                } else {
                    button.className = 'mt-3 inline-flex w-full justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50 sm:mt-0 sm:w-auto';
                }

                button.addEventListener('click', () => {
                    if (action.action) {
                        const result = action.action();
                        if (result !== false) {
                            this.hide(action.value || action.text);
                        }
                    } else {
                        this.hide(action.value || action.text);
                    }
                });

                actionsContainer.appendChild(button);
            });
        }

        handleKeyDown(e) {
            if (e.key === 'Escape') {
                this.hide();
            } else if (e.key === 'Tab') {
                this.handleTab(e);
            }
        }

        trapFocus() {
            this.focusableElements = this.modal.querySelectorAll(
                'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
            );

            if (this.focusableElements.length > 0) {
                this.focusableElements[0].focus();
            }
        }

        handleTab(e) {
            const firstFocusable = this.focusableElements[0];
            const lastFocusable = this.focusableElements[this.focusableElements.length - 1];

            if (e.shiftKey) {
                if (document.activeElement === firstFocusable) {
                    e.preventDefault();
                    lastFocusable.focus();
                }
            } else {
                if (document.activeElement === lastFocusable) {
                    e.preventDefault();
                    firstFocusable.focus();
                }
            }
        }
    }

    // Create singleton instance
    const modalManager = new ModalManager();

    // Public API
    window.showModal = function(options) {
        return modalManager.show(options);
    };

    window.hideModal = function(result) {
        modalManager.hide(result);
    };

    // Convenience methods
    window.showSuccess = function(message, title = 'Success') {
        return showModal({
            title: title,
            message: message,
            type: 'success'
        });
    };

    window.showError = function(message, title = 'Error') {
        return showModal({
            title: title,
            message: message,
            type: 'error'
        });
    };

    window.showWarning = function(message, title = 'Warning') {
        return showModal({
            title: title,
            message: message,
            type: 'warning'
        });
    };

    window.showInfo = function(message, title = 'Information') {
        return showModal({
            title: title,
            message: message,
            type: 'info'
        });
    };

    window.showConfirm = function(message, title = 'Confirm') {
        return showModal({
            title: title,
            message: message,
            type: 'confirm',
            actions: [
                { text: 'Cancel', action: () => hideModal(false) },
                { text: 'Confirm', primary: true, action: () => hideModal(true) }
            ]
        });
    };

})(window);