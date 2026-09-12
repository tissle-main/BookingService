window.bookingServiceAuth = {
    login: async function (command) {
        try {
            const response = await fetch("/api/auth/login", {
                method: "POST",
                credentials: "same-origin",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(command)
            });

            if (response.ok) {
                return { success: true, message: null };
            }

            const problem = await response.json().catch(() => null);
            return {
                success: false,
                message: problem?.detail ?? problem?.title ?? "Invalid email or password."
            };
        } catch {
            return { success: false, message: "Unable to connect to the server." };
        }
    },

    logout: async function () {
        try {
            const response = await fetch("/api/auth/logout", {
                method: "POST",
                credentials: "same-origin"
            });
            return response.ok;
        } catch {
            return false;
        }
    }
};
